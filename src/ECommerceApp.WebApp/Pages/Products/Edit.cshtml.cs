using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using ECommerceApp.Common.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using ECommerceApp.Domain.Entities;

namespace ECommerceApp.WebApp.Pages.Products;

public class EditModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EditModel> _logger;

    public EditModel(IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<EditModel> logger)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    [BindProperty]
    public ProductDto Product { get; set; } = new();

    // This property will hold the dynamically loaded categories
    public SelectList CategoryOptions { get; set; } = new(Enumerable.Empty<string>());
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
        var categoriesResponse = await client.GetAsync($"{apiUrl}/api/Categories");
        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var categoriesJsonResponse = await categoriesResponse.Content.ReadAsStringAsync();
            var productDto = JsonSerializer.Deserialize<ProductDto>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var categories = JsonSerializer.Deserialize<List<CategoryDto>>(categoriesJsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            CategoryOptions = new SelectList(categories, nameof(Category.Id), nameof(Category.Name), "Please Select");
            if (productDto != null)
            {
                Product = new ProductDto
                {
                    Id = productDto.Id,
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    Quantity = productDto.Quantity,
                    CategoryId = productDto.CategoryId
                };
                return Page();
            }
        }

        ErrorMessage = "Product not found.";
        return RedirectToPage("/Products/Index");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Identity/Account/Login");

        var client = _clientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var apiUrl = _configuration["ApiBaseUrl"];

        var productCommand = new
        {
            Product.Id,
            Product.Name,
            Product.Description,
            Product.Price,
            Product.Quantity,
            Product.CategoryId
        };

        var content = new StringContent(JsonSerializer.Serialize(productCommand), Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"{apiUrl}/api/Products/{Product.Id}", content);

        if (response.IsSuccessStatusCode)
            return RedirectToPage("/Products/Index");

        ErrorMessage = "Failed to update product.";
        _logger.LogError("Failed to update product. Status code: {StatusCode}", response.StatusCode);
        return Page();
    }
}