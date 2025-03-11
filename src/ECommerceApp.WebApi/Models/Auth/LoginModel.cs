using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.WebApi.Models.Auth;

public class LoginModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;

    public bool RememberMe { get; set; }
}
