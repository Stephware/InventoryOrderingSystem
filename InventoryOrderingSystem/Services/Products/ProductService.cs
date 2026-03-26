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
        {
            if (string.IsNullOrWhiteSpace(product.ProductName))
                throw new Exception("Product name required");

            if (product.Price <= 0)
                throw new Exception("Price must be greater than 0");

            if (product.Stock < 0)
                throw new Exception("Stock cannot be negative");

            await _productRepo.AddAsync(product);
        }

        public async Task UpdatePriceAsync(int id, decimal price)
        {
            if (price <= 0)
                throw new InvalidOperationException("Price must be greater than zero.");

            var product = await _productRepo.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Product not found.");

            product.Price = price;
            await _productRepo.UpdateAsync(product);
        }

        public async Task UpdateStockAsync(int id, int stock)
        {
            if (stock < 0)
                throw new InvalidOperationException("Stock cannot be negative.");

            var product = await _productRepo.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Product not found.");

            product.Stock = stock;
            await _productRepo.UpdateAsync(product);
        }
    }
}