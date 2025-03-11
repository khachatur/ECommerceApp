using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.WebApi.Models.Auth;

public class RegisterModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = default!;

    [Required]
    [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = default!;
}
