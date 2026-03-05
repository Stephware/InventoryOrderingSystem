using InventoryOrderingSystem.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryOrderingSystem.Repository.Customers
{
    public class CustomerRepository
    {
        private readonly InventoryOrderingSystemContext _context;

        public CustomerRepository(InventoryOrderingSystemContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetbyIdAsync(int customerId)
        {
            return await _context.Customers
                .Include(x=>x.CustomerId)
                .FirstOrDefaultAsync();
        }
    }
}
