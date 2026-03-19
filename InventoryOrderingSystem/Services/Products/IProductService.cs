using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Services.Products
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task UpdateStockAsync(int id, int stock);
        Task AddAsync(Product product);
        Task UpdatePriceAsync(int id, decimal price);
    }
}