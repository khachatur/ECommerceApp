using ECommerceApp.Domain.Entities;

namespace ECommerceApp.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
    }
}
