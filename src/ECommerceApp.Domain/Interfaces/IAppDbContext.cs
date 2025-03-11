using ECommerceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ECommerceApp.Domain.Interfaces;

public interface IAppDbContext
{
    DbSet<Product> Products { get; set; }
    DbSet<Category> Categories { get; set; }
    DbSet<Order> Orders { get; set; }
    DbSet<OrderItem> OrderItems { get; set; }
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task UpdateProductStockAsync(int productId, int quantity);
}
