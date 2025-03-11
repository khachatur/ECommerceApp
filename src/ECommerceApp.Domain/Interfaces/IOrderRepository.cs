using ECommerceApp.Domain.Entities;

namespace ECommerceApp.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
        Task AddAsync(Order order);
    }
}
