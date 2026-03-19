using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Repository.Orders
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);

    }
}
