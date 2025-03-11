using ECommerceApp.Common.DTOs;
using MediatR;

namespace ECommerceApp.Application.Features.Orders.Commands
{
    public class CreateOrderCommand : IRequest<int>
    {
        public string UserId { get; set; } = string.Empty;
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
}
