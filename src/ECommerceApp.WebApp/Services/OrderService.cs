using ECommerceApp.Common.DTOs;
using System.Net.Http.Headers;

namespace ECommerceApp.WebApp.Services;

public class OrderService : IOrderService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<OrderService> logger)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<List<OrderDto>> GetOrdersAsync(string token)
    {
        _logger.LogInformation("Fetching all orders");
        var client = CreateClient(token);
        var apiUrl = _configuration["ApiBaseUrl"];
        var response = await client.GetAsync($"{apiUrl}/api/orders");
        if (response != null && response.IsSuccessStatusCode)
        {
            var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();
            if (orders != null)
            {
                _logger.LogInformation("Successfully fetched {OrderCount} orders", orders.Count);
                return orders;
            }
        }
        _logger.LogError("Failed to fetch orders. Status: {StatusCode}", response?.StatusCode);
        return new List<OrderDto>();
    }

    public async Task<OrderDto> GetOrderAsync(int id, string token)
    {
        _logger.LogInformation("Fetching order with ID: {OrderId}", id);
        var client = CreateClient(token);
        var apiUrl = _configuration["ApiBaseUrl"];
        var response = await client.GetAsync($"{apiUrl}/api/orders/{id}");
        if (response != null && response.IsSuccessStatusCode)
        {
            var order = await response.Content.ReadFromJsonAsync<OrderDto>();
            if (order != null)
            {
                _logger.LogInformation("Successfully fetched order with ID: {OrderId}", id);
                return order;
            }
        }
        else
        {
            _logger.LogError("Failed to fetch order with ID: {OrderId}. Status: {StatusCode}", id, response?.StatusCode);
        }
        return new OrderDto();
    }

    public async Task<int> CreateOrderAsync(List<OrderItemDto> orderItems, string token)
    {
        _logger.LogInformation("Creating order with {ItemCount} items", orderItems.Count);
        var client = CreateClient(token);
        var apiUrl = _configuration["ApiBaseUrl"];
        var request = new { OrderItems = orderItems.Where(oi => oi.Quantity > 0).ToList() };
        var response = await client.PostAsJsonAsync($"{apiUrl}/api/orders", request);
        if (response.IsSuccessStatusCode)
        {
            var orderId = await response.Content.ReadFromJsonAsync<int>();
            _logger.LogInformation("Order created successfully with ID: {OrderId}", orderId);
            return orderId;
        }
        _logger.LogError("Failed to create order. Status: {StatusCode}", response.StatusCode);
        return -1;
    }

    private HttpClient CreateClient(string token)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
