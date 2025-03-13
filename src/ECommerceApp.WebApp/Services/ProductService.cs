using ECommerceApp.Common.DTOs;
using System.Net.Http.Headers;

namespace ECommerceApp.WebApp.Services;

public class ProductService : IProductService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProductService> _logger;
    public ProductService(IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<ProductService> logger)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<List<ProductDto>> GetProductsAsync(string token)
    {
        var client = CreateClient(token);
        var apiUrl = _configuration["ApiBaseUrl"];
        var response = await client.GetAsync($"{apiUrl}/api/products");
        if (response.IsSuccessStatusCode)
        {
            var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
            if (products != null)
            {
                _logger.LogInformation("Successfully fetched {ProductCount} products", products.Count);
                return products;
            }
        }
        _logger.LogError("Failed to fetch products. Status: {StatusCode}", response.StatusCode);
        return new List<ProductDto>();
    }

    public async Task<ProductDto> GetProductAsync(int id, string token)
    {
        var client = CreateClient(token);
        var apiUrl = _configuration["ApiBaseUrl"];
        var response = await client.GetAsync($"{apiUrl}/api/orders/ts/{id}");
        if (response != null && response.IsSuccessStatusCode)
        {
            var product = await response.Content.ReadFromJsonAsync<ProductDto>();
            if (product != null)
            {
                _logger.LogInformation("Successfully fetched product with ID: {ProductId}", id);
                return product;
            }
        }
        else
        {
            _logger.LogError("Failed to fetch product with ID: {ProductId}. Status: {StatusCode}", id, response?.StatusCode);
        }
        return new ProductDto();
    }

    public async Task CreateProductAsync(ProductDto product, string token)
    {
        _logger.LogInformation("Creating product: {ProductName}", product.Name);
        var client = CreateClient(token);
        var apiUrl = _configuration["ApiBaseUrl"];
        var response = await client.PostAsJsonAsync($"{apiUrl}/api/products", product);
        if (response != null && response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Product created successfully: {ProductName}", product.Name);
        }
        else
        {
            _logger.LogError("Failed to create product: {ProductName}. Status: {StatusCode}", product.Name, response?.StatusCode);
        }
    }

    public async Task UpdateProductAsync(ProductDto product, string token)
    {
        _logger.LogInformation("Updating product with ID: {ProductId}", product.Id);
        var client = CreateClient(token);
        var apiUrl = _configuration["ApiBaseUrl"];
        var response = await client.PutAsJsonAsync($"{apiUrl}/api/products/{product.Id}", product);
        if (response != null && response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Product updated successfully: {ProductId}", product.Id);
        }
        else
        {
            _logger.LogError("Failed to update product with ID: {ProductId}. Status: {StatusCode}", product.Id, response?.StatusCode);
        }
    }

    public async Task DeleteProductAsync(int id, string token)
    {
        _logger.LogInformation("Deleting product with ID: {ProductId}", id);
        var client = CreateClient(token);
        var apiUrl = _configuration["ApiBaseUrl"];
        var response = await client.DeleteAsync($"{apiUrl}/api/products/{id}");
        if (response != null && response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Product deleted successfully: {ProductId}", id);
        }
        else
        {
            _logger.LogError("Failed to delete product with ID: {ProductId}. Status: {StatusCode}", id, response?.StatusCode);
        }
    }

    private HttpClient CreateClient(string token)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}