using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Infrastructure;

public class AppDbContext : IdentityDbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Products = Set<Product>();
        Categories = Set<Category>();
        Orders = Set<Order>();
        OrderItems = Set<OrderItem>();
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);
        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId);
        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateProductStockAsync(int productId, int quantity)
    {
        await Database.ExecuteSqlRawAsync(
            "EXEC UpdateProductStock @ProductId, @Quantity",
            new SqlParameter("@ProductId", productId),
            new SqlParameter("@Quantity", quantity));
    }
}
