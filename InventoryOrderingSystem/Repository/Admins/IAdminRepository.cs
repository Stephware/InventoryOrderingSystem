using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Repository.Admins
{
    public interface IAdminRepository
    {
        Task<Admin?> GetByUsernameAsync(string username);
    }
}
