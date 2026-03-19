using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Services.Products
{
    public interface IProductService
    {
        Task<Product?> GetProductByIdAsync(int productId);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task UpdateProductAsync(Product product);
    }
}