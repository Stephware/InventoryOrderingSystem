using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repository.Admins;
using System.Security.Cryptography;
using System.Text;

namespace InventoryOrderingSystem.Services.Admins
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repo;

        public AdminService(IAdminRepository repo)
        {
            _repo = repo;
        }

        public async Task<Admin?> LoginAsync(string username, string password)
        {
            var admin = await _repo.GetByUsernameAsync(username);

            if (admin == null) return null;

            var hash = HashPassword(password);

            return admin.PasswordHash == hash ? admin : null;
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
