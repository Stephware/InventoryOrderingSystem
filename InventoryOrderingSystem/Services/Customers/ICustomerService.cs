using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Services.Customers
{
    public interface ICustomerService
    {
        Task<Customer?> GetCustomerDetailsAsync(int customerId);
    }
}