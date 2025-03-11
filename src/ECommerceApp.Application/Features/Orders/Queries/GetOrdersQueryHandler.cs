using ECommerceApp.Common.DTOs;
using ECommerceApp.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Application.Features.Orders.Queries
{
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<OrderDto>>
    {
        private readonly IAppDbContext _context;

        public GetOrdersQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == request.UserId)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        ProductName = oi.Product.Name
                    }).ToList()
                })
                .ToListAsync();
        }
    }
}
