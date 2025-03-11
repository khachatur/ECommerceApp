using ECommerceApp.Common.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ECommerceApp.WebApp.Pages;

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
        var client = _clientFactory.CreateClient();
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

    public async Task<IActionResult> OnGetBuyAsync(int productId)
    {
        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Account/Login", new { area = "Identity" }); // Redirect to login if not authenticated

        var client = _clientFactory.CreateClient("ApiClient");
        var apiUrl = _configuration["ApiBaseUrl"];
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var orderRequest = new
        {
            OrderItems = new List<object> { new { ProductId = productId, Quantity = 1 } }
        };
        var response = await client.PostAsJsonAsync($"{apiUrl}/api/orders", orderRequest);
        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Order placed successfully!";
            return RedirectToPage("/Orders/Index");
        }

        TempData["Error"] = "Failed to place order.";
        return RedirectToPage();
    }

    public IActionResult OnPostLogout()
    {
        HttpContext.Session.Remove("JwtToken");
        return RedirectToPage("/Index");
    }
}
