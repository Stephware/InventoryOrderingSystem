using InventoryOrderingSystem.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryOrderingSystem.Repository.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly InventoryOrderingSystemContext _context;

        public OrderRepository (InventoryOrderingSystemContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }
    }
}
