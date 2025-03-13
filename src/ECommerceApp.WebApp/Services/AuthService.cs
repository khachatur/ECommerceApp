using System.Text.Json;
using System.Text;
using static ECommerceApp.WebApp.Areas.Identity.Pages.Account.LoginModel;

namespace ECommerceApp.WebApp.Services;

public class AuthService : IAuthService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> LoginAsync(string email, string password, bool rememberMe)
    {
        var client = _clientFactory.CreateClient();
        var loginData = new { email, password, rememberMe };
        var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

        var apiUrl = _configuration["ApiBaseUrl"];
        var response = await client.PostAsync($"{apiUrl}/api/Auth/login", content);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result != null)
            {
                _logger.LogInformation("Login successful for user: {input.Email}", email);
                return result.Token;
            }
            else
            {
                _logger.LogWarning("Login failed for user: {input.Email}", email);
            }
        }
        return string.Empty;
    }

    public async Task<bool> RegisterAsync(string email, string password, string confirmPassword)
    {
        _logger.LogInformation("Attempting registration for email: {Email}", email);
        var client = _clientFactory.CreateClient("ApiClient");
        var registrationData = new { email, password, confirmPassword };
        var content = new StringContent(JsonSerializer.Serialize(registrationData), Encoding.UTF8, "application/json");
        var apiUrl = _configuration["ApiBaseUrl"];
        var response = await client.PostAsJsonAsync($"{apiUrl}/api/Auth/register", new { Email = email, Password = password, ConfirmPassword = confirmPassword });
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Registration successful for email: {Email}", email);
            return response.IsSuccessStatusCode;
        }
        _logger.LogError("Registration failed for email: {Email}. Status: {StatusCode}", email, response.StatusCode);
        return false;
    }
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
}

