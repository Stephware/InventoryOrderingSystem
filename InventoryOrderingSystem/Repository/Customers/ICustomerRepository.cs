using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Repository.Customers
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int customerId); 
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task ValidateUser(string username, string passwordHash);
    }
}
