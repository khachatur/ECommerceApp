using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Infrastructure
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IAppDbContext _context;

        public CategoryRepository(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync() => await _context.Categories.ToListAsync();
    }
}
