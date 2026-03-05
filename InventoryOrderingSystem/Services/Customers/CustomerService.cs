using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repository.Customers;

namespace InventoryOrderingSystem.Services.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepo;
        public CustomerService(ICustomerRepository customerRepo) => _customerRepo = customerRepo;
        public async Task<Customer?> GetCustomerDetailsAsync(int customerId) => await _customerRepo.GetByIdAsync(customerId);
    }
}