using System.ComponentModel.DataAnnotations;
using ECommerceApp.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECommerceApp.WebApp.Areas.Identity.Pages.Account;
public class RegisterModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(IAuthService authService, ILogger<RegisterModel> logger)
    {
        _authService = authService;
        _logger = logger;
        Input = new InputModel();
        ReturnUrl = string.Empty;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public void OnGet(string? returnUrl)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl)
    {
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var success = await _authService.RegisterAsync(Input.Email, Input.Password, Input.ConfirmPassword);
            if (success)
            {
                _logger.LogInformation("User registered.");
                return LocalRedirect(returnUrl);
            }
            ModelState.AddModelError(string.Empty, "Registration failed.");
        }

        return Page();
    }
}

