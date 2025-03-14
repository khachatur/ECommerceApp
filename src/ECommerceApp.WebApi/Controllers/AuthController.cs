﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ECommerceApp.WebApi.Models.Auth;
using ECommerceApp.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ECommerceApp.Common.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ECommerceApp.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly TokenProvider _tokenProvider;

    public AuthController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        TokenProvider tokenProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenProvider = tokenProvider;
    }

    // POST: api/Auth/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if user already exists
        var userExists = await _userManager.FindByEmailAsync(model.Email);
        if (userExists is not null)
            return StatusCode(StatusCodes.Status409Conflict, new { Status = "Error", Message = "User already exists!" });

        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed!", Errors = errors });
        }

        result = await _userManager.AddToRoleAsync(user, "User");
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Failed to add user to role!", Errors = errors });
        }

        return Ok(new { Status = "Success", Message = "User created successfully!" });
    }

    // POST: api/Auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if the user exists
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Unauthorized(new { Message = "Invalid credentials" });

        // Validate the password
        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new { Message = "Invalid credentials" });

        // Get user roles
        var userRoles = await _userManager.GetRolesAsync(user);

        // Create JWT token
        var token = _tokenProvider.Create(user, userRoles);

        return Ok(new { token });
    }

    // GET: api/Auth/users
    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        var userInfos = new List<UserInfo>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userInfos.Add(new UserInfo { UserId = user.Id, Email = user.Email, Roles = roles.ToList() });
        }
        return Ok(userInfos);
    }
}
