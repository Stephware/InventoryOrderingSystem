using InventoryOrderingSystem.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryOrderingSystem.Repository.Customers
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly InventoryOrderingSystemContext _context;

        public CustomerRepository(InventoryOrderingSystemContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByIdAsync(int customerId)
        {
            return await _context.Customers
                .FirstOrDefaultAsync();
        }


    }
}
