using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Infrastructure;

public class ProductRepository : IProductRepository
{
    private readonly IAppDbContext _context;

    public ProductRepository(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Product> GetByIdAsync(int id) =>
        await _context.Products.FirstOrDefaultAsync(p => p.Id == id) ?? throw new Exception("Product not found");

    public async Task<IEnumerable<Product>> GetAllAsync() => await _context.Products.Include(p => p.Category).ToListAsync();

    public async Task AddAsync(Product product) => await _context.Products.AddAsync(product);

    public async Task UpdateAsync(Product product) => _context.Products.Update(product);

    public async Task DeleteAsync(int id)
    {
        var product = await GetByIdAsync(id);
        _context.Products.Remove(product);
    }
}