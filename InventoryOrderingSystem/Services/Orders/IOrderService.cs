using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Services.Orders
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int orderId);
        Task CreateOrderAsync(
            int customerId,
            List<(int productId, int qty, decimal price)> items
        );
        Task<Order> UpdateStatusAsync(int orderId, string status);
        Task CancelOrderAsync(int orderId);
        Task UpdateOrderAsync(Order order);
    }
}