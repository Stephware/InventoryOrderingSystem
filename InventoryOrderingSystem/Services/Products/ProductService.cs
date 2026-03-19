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

        public async Task<List<Product>> GetAllProductsAsync()
                => await _productRepo.GetAllAsync();

        public async Task AddAsync(Product product)
            => await _productRepo.AddAsync(product);

        public async Task UpdatePriceAsync(int id, decimal price)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return;

            product.Price = price;
            await _productRepo.UpdateAsync(product);
        }

        public async Task UpdateStockAsync(int id, int stock)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return;

            product.Stock = stock;
            await _productRepo.UpdateAsync(product);
        }
    }
}