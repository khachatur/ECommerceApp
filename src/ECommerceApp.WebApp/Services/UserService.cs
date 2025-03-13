using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace ECommerceApp.WebApp.Services;

public class UserService : IUserService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserService> _logger;

    public UserService(IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<UserService> logger)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public Task<UserInfo> GetCurrentUserAsync(HttpContext httpContext)
    {
        var token = httpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
            return Task.FromResult<UserInfo>(null!);

        _logger.LogDebug("Retrieving user info from JWT token");
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? string.Empty;
        var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty;
        var roles = jwtToken.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();

        var userInfo = new UserInfo { UserId = userId, Email = email, Roles = roles };
        _logger.LogDebug("User info retrieved: {Email}", email);
        return Task.FromResult(userInfo);
    }
    public async Task<List<UserInfo>> GetUsersAsync(string adminToken)
    {
        _logger.LogInformation("Fetching all users with admin token");
        var client = CreateClient(adminToken);
        var apiUrl = _configuration["ApiBaseUrl"];
        var response = await client.GetAsync($"{apiUrl}/api/Auth/users");
        if (response.IsSuccessStatusCode)
        {
            var users = await response.Content.ReadFromJsonAsync<List<UserInfo>>();
            if (users != null)
            {
                _logger.LogInformation("Successfully fetched {UserCount} users", users.Count);
                return users;
            }
            _logger.LogError("Failed to fetch users. The response content was null.");
        }
        else
        {
            _logger.LogError("Failed to fetch users. Status: {StatusCode}", response.StatusCode);
        }
        return new List<UserInfo>();
    }

    private HttpClient CreateClient(string token)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}

public class UserInfo
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
}