using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Interfaces;
using MediatR;
using ECommerceApp.Infrastructure;

namespace ECommerceApp.Application.Features.Orders.Commands
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly AppDbContext _context; // Use concrete type for Database access

        public CreateOrderCommandHandler(IOrderRepository orderRepository, AppDbContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }

        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            // Validate and prepare order items
            foreach (var item in request.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.Quantity < item.Quantity)
                    throw new Exception($"Product {item.ProductId} not found or insufficient stock.");

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price // Set UnitPrice from product
                };
                orderItems.Add(orderItem);
                totalAmount += product.Price * item.Quantity;
            }

            var order = new Order
            {
                UserId = request.UserId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                OrderItems = orderItems
            };

            // Use transaction for data consistency
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Add order via repository
                await _orderRepository.AddAsync(order);

                // Update stock using stored procedure
                foreach (var item in orderItems)
                {
                    await _context.UpdateProductStockAsync(item.ProductId, item.Quantity);
                }

                await transaction.CommitAsync(cancellationToken);
                return order.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new Exception("Failed to create order.", ex);
            }
        }
    }
}
