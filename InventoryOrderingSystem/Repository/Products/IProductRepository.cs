using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Repository.Products
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int productId);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
    }
}
