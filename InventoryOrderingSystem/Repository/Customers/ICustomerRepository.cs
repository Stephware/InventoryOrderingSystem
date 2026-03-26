using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Repository.Customers
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int customerId); 
        Task<Customer?> GetUsernameAsync (string username);
        Task<Customer?> ValidateUser(string username, string passwordHash);
        Task RegisterUser(Customer customer);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
    }
}
