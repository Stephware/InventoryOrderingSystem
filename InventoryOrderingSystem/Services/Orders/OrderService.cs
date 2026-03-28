using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repository.Customers;
using InventoryOrderingSystem.Repository.Orders;
using InventoryOrderingSystem.Repository.Products;
using Microsoft.EntityFrameworkCore;

namespace InventoryOrderingSystem.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly InventoryOrderingSystemContext _context;

        public OrderService(
            IOrderRepository orderRepo,
            IProductRepository productRepo,
            ICustomerRepository customerRepo,
            InventoryOrderingSystemContext context)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _customerRepo = customerRepo;
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync()
            => await _orderRepo.GetAllAsync();

        public async Task<Order?> GetByIdAsync(int orderId)
            => await _orderRepo.GetByIdAsync(orderId);

        public async Task CancelOrderAsync(int orderId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _orderRepo.GetByIdAsync(orderId);
                if (order == null)
                    throw new Exception("Order not found.");

                if (order.Status == "Completed")
                    throw new Exception("Cannot cancel a completed order.");

                order.Status = "Cancelled";
                await _orderRepo.UpdateAsync(order);

                foreach (var item in order.OrderItems)
                {
                    var product = await _productRepo.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                        await _productRepo.UpdateAsync(product);
                    }
                }

                order.Status = "Cancelled";
                await _orderRepo.UpdateAsync(order);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateOrderAsync(Order order)
            => await _orderRepo.UpdateAsync(order);

        public async Task <int> CreateOrderAsync(int customerId, List<(int productId, int qty)> items)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Constraint 1
                var customer = await _customerRepo.GetByIdAsync(customerId);
                if (customer == null)
                    throw new Exception("Customer does not exist.");

                // Constraint 2
                if (!customer.IsActive)
                    throw new Exception("Customer is not active.");

                if (items == null || !items.Any())
                    throw new Exception("Order must contain at least one item.");

                var order = new Order
                {
                    CustomerId = customerId,
                    Status = "Pending",
                    DateCreated = DateTime.Now,
                    OrderItems = new List<OrderItem>()
                };

                decimal total = 0;

                foreach (var item in items)
                {
                    var product = await _productRepo.GetByIdAsync(item.productId);
                    if (product == null)
                        throw new Exception("Product does not exist.");

                    // Constraint 5
                    if (product.Stock <= 0)
                        throw new Exception($"{product.ProductName} is out of stock.");

                    if (product.Stock < item.qty)
                        throw new Exception($"Not enough stock for {product.ProductName}. Available: {product.Stock}");

                    // Constraint 4
                    product.Stock -= item.qty;
                    await _productRepo.UpdateAsync(product);

                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = product.ProductId,
                        Quantity = item.qty,
                        Price = product.Price
                    });

                    // Constraint 3
                    total += product.Price * item.qty;
                }

                order.TotalAmount = total;

                await _orderRepo.AddAsync(order);

                await transaction.CommitAsync();

                return order.OrderId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ✅ UPDATE STATUS
        public async Task<Order?> UpdateStatusAsync(int orderId, string status)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return null;

            order.Status = status;
            await _orderRepo.UpdateAsync(order);
            return order;
        }

        public async Task UpdateOrderItemsAsync(int orderId, List<(int productId, int qty)> newItems)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _orderRepo.GetByIdAsync(orderId);
                if (order == null)
                    throw new Exception("Order not found.");

                if (order.Status == "Cancelled")
                    throw new Exception("Cannot modify cancelled order.");

                if (order.Status == "Completed")
                    throw new Exception("Cannot modify completed order.");

                if (newItems == null || !newItems.Any())
                    throw new Exception("Order must contain at least one item.");

                foreach (var oldItem in order.OrderItems)
                {
                    var product = await _productRepo.GetByIdAsync(oldItem.ProductId);
                    if (product != null)
                    {
                        product.Stock += oldItem.Quantity;
                        await _productRepo.UpdateAsync(product);
                    }
                }

                // Remove existing order items properly
                _context.OrderItems.RemoveRange(order.OrderItems);
                await _context.SaveChangesAsync();

                decimal total = 0;

                foreach (var item in newItems)
                {
                    var product = await _productRepo.GetByIdAsync(item.productId);

                    if (product == null)
                        throw new Exception("Product not found.");

                    if (product.Stock <= 0)
                        throw new Exception($"{product.ProductName} is out of stock.");

                    if (product.Stock < item.qty)
                        throw new Exception($"Not enough stock for {product.ProductName}");

                    product.Stock -= item.qty;

                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = product.ProductId,
                        Quantity = item.qty,
                        Price = product.Price
                    });

                    total += product.Price * item.qty;

                    await _productRepo.UpdateAsync(product);
                }

                order.TotalAmount = total;

                await _orderRepo.UpdateAsync(order);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}