using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repository.Products;

namespace InventoryOrderingSystem.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;

        public ProductService(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _productRepo.GetByIdAsync(productId);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepo.GetAllAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepo.UpdateAsync(product);
        }
    }
}