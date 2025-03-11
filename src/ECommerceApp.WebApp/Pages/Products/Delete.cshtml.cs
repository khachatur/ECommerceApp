using ECommerceApp.Common.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ECommerceApp.WebApp.Pages.Products;

public class DeleteModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<DeleteModel> logger)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    [BindProperty]
    public ProductDto Product { get; set; } = new();

    public string ErrorMessage { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Identity/Account/Login");

        var client = _clientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var apiUrl = _configuration["ApiBaseUrl"];
        var response = await client.GetAsync($"{apiUrl}/api/Products/{id}");

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Product = JsonSerializer.Deserialize<ProductDto>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new ProductDto();
            return Page();
        }
        else
        {
            ErrorMessage = "Product not found.";
            return RedirectToPage("/Products/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Identity/Account/Login");

        var client = _clientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var apiUrl = _configuration["ApiBaseUrl"];
        var response = await client.DeleteAsync($"{apiUrl}/api/Products/{Product.Id}");

        if (response.IsSuccessStatusCode)
            return RedirectToPage("/Products/Index");

        ErrorMessage = "Failed to delete product.";
        _logger.LogError("Failed to delete product. Status code: {StatusCode}", response.StatusCode);
        return Page();
    }
}
