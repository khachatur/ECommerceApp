using ECommerceApp.Common.DTOs;

namespace ECommerceApp.WebApp.Services;

public interface IOrderService
{
    Task<List<OrderDto>> GetOrdersAsync(string token);
    Task<OrderDto> GetOrderAsync(int id, string token);
    Task<int> CreateOrderAsync(List<OrderItemDto> orderItems, string token);
}
