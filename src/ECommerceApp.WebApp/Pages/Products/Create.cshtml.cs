using ECommerceApp.Common.DTOs;
using ECommerceApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ECommerceApp.WebApp.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<CreateModel> logger)
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

        public async Task OnGetAsync()
        {
            // Get JWT token from session
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                Response.Redirect("/Identity/Account/Login");
                return;
            }

            CategoryOptions = await GetCategoryOptions();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) { 
                CategoryOptions = await GetCategoryOptions();
                return Page();
            }

            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Identity/Account/Login");

            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var apiUrl = _configuration["ApiBaseUrl"];

            var productCommand = new
            {
                Product.Name,
                Product.Description,
                Product.Price,
                Product.Quantity,
                Product.CategoryId
            };

            var content = new StringContent(JsonSerializer.Serialize(productCommand), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{apiUrl}/api/Products", content);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("/Products/Index");

            ErrorMessage = "Failed to create product.";
            _logger.LogError("Failed to create product. Status code: {StatusCode}", response.StatusCode);
            return Page();
        }

        private async Task<SelectList> GetCategoryOptions()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return new SelectList(Enumerable.Empty<string>());
            }

            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var apiUrl = _configuration["ApiBaseUrl"];
            var response = await client.GetAsync($"{apiUrl}/api/Categories");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<CategoryDto>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                CategoryOptions = new SelectList(categories, nameof(Category.Id), nameof(Category.Name), "Please Select");
            }
            else
            {
                CategoryOptions = new SelectList(new List<string>());
                _logger.LogError("Failed to load categories. Status code: {StatusCode}", response.StatusCode);
            }

            return CategoryOptions;
        }
    }
}
