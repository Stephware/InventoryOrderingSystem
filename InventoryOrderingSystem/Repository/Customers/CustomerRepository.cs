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

        public async Task<List<Customer>> GetAllAsync()
            => await _context.Customers.ToListAsync();

        public async Task AddAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<Customer?> GetByIdAsync(int customerId)
        {
            return await _context.Customers.FindAsync(customerId);
        }

        public Task ValidateUser(string username, string passwordHash)
        {
            throw new NotImplementedException();
            //return _context.Customers.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash && u.IsActive);
        }
    }
}
