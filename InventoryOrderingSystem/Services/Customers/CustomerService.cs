using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repository.Customers;

namespace InventoryOrderingSystem.Services.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repo;

        public CustomerService(ICustomerRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Customer>> GetAllAsync()
            => await _repo.GetAllAsync();

        public async Task AddAsync(Customer customer)
            => await _repo.AddAsync(customer);

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Customer customer)
        {
            await _repo.UpdateAsync(customer);
        }
    }
}