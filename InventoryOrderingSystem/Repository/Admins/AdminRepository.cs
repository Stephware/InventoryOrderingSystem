using InventoryOrderingSystem.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryOrderingSystem.Repository.Admins
{
    public class AdminRepository : IAdminRepository
    {
        private readonly InventoryOrderingSystemContext _context;

        public AdminRepository(InventoryOrderingSystemContext context)
        {
            _context = context;
        }

        public async Task<Admin?> GetByUsernameAsync(string username)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(a => a.Username == username);
        }
    }
}
