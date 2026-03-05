using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Repository.Customers
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int customerId);
    }
}
