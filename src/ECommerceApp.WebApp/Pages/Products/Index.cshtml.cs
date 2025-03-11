using ECommerceApp.Common.DTOs;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ECommerceApp.WebApp.Pages.Products;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<IndexModel> logger)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public List<ProductDto> Products { get; set; } = new();

    public async Task OnGetAsync()
    {
        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
        {
            Response.Redirect("/Identity/Account/Login");
            return;
        }

        var client = _clientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var apiUrl = _configuration["ApiBaseUrl"];
        var response = await client.GetAsync($"{apiUrl}/api/Products");

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Products = JsonSerializer.Deserialize<List<ProductDto>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ProductDto>();
        }
        else
        {
            _logger.LogError("Failed to fetch products. Status code: {StatusCode}", response.StatusCode);
        }
    }
}
