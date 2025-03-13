using static ECommerceApp.WebApp.Areas.Identity.Pages.Account.LoginModel;

namespace ECommerceApp.WebApp.Services;

public interface IAuthService
{
    Task<string> LoginAsync(string email, string password, bool rememberMe);
    Task<bool> RegisterAsync(string email, string password, string confirmPassword);
}
