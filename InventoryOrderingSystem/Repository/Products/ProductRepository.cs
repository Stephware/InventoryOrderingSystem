using InventoryOrderingSystem.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryOrderingSystem.Repository.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly InventoryOrderingSystemContext _context;

        public ProductRepository(InventoryOrderingSystemContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
         => await _context.Products.ToListAsync();

        public async Task<Product?> GetByIdAsync(int id)
            => await _context.Products.FindAsync(id);

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}
