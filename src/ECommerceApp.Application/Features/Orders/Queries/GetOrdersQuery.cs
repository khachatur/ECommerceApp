using ECommerceApp.Common.DTOs;
using MediatR;

namespace ECommerceApp.Application.Features.Orders.Queries
{
    public class GetOrdersQuery : IRequest<List<OrderDto>>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
