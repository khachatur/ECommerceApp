using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Infrastructure;

public class OrderRepository : IOrderRepository
{
    private readonly IAppDbContext _context;

    public OrderRepository(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Order> GetByIdAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order ?? throw new Exception("Order not found");
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync(CancellationToken.None);
    }
}
