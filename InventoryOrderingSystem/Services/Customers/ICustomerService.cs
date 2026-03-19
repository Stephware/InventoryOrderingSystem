using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Services.Customers
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllAsync();

        Task<Customer?> GetByIdAsync(int id);

        Task AddAsync(Customer customer);

        Task UpdateAsync(Customer customer);

    }
}