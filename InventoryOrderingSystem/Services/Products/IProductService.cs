using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Services.Products
{
    public interface IProductService
    {
        Task<Product?> GetProductByIdAsync(int productId);
    }
}