using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Repository.Products
{
    public class ProductRepository
    {
        private readonly InventoryOrderingSystemContext _context;

        public ProductRepository(InventoryOrderingSystemContext context)
        {
            _context = context;
        }
        public async Task<Product?> GetbyIdAsync(int id)
        {
            return await _context.Products
                .FindAsync(id);
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}
