using ECommerceApp.WebApp.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.WebApp.Pages.Users;

public class IndexModel : PageModel
{
    private readonly IUserService _userService;

    public IndexModel(IUserService userService)
    {
        _userService = userService;
    }

    public List<UserInfo> Users { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userInfo = await _userService.GetCurrentUserAsync(HttpContext);
        if (userInfo == null || !userInfo.Roles.Contains("Admin"))
            return RedirectToPage("/Index");

        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Index");

        Users = await _userService.GetUsersAsync(token);
        return Page();
    }
}