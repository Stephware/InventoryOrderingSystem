using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Repository.Orders
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
    }
}
