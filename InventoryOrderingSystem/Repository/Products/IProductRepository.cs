using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Repository.Products
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int productId);
        Task UpdateAsync(Product product);
        Task<IEnumerable<Product>> GetAllAsync();
    }
}
