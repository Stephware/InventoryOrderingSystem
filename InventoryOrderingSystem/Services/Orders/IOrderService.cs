using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Services.Orders
{
    public interface IOrderService
    {
        Task<Order> PlaceOrderAsync(int customerId, int productId, int quantity);
    }
}