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

        public OrderService(IOrderRepository orderRepo, IProductRepository productRepo, ICustomerRepository customerRepo)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _customerRepo = customerRepo;
        }

        public async Task<List<Order>> GetAllAsync()
            => await _orderRepo.GetAllAsync();

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _orderRepo.GetByIdAsync(orderId);
        }

        public async Task CancelOrderAsync(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return;

            order.Status = "Cancelled";
            await _orderRepo.UpdateAsync(order);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await _orderRepo.UpdateAsync(order);
        }

        public async Task CreateOrderAsync(int customerId, List<(int productId, int qty, decimal price)> items)
        {
            //Constraint 1. Customer must exist
            var customer = await _customerRepo.GetByIdAsync(customerId);
            if (customer == null)
                throw new Exception("Customer does not exist.");

            //Constraint 2. Customer must be active before ordering
            if (!customer.IsActive)
                throw new InvalidOperationException("Customer is not active.");

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

                //Constraint 5. Customers cannot order once product stock is 0
                if (product.Stock <= 0 || product.Stock < item.qty)
                    throw new Exception("Insufficient stock.");

                //Constraint 4. Product stock must be reduced after successful order
                product.Stock -= item.qty;
                await _productRepo.UpdateAsync(product);

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.productId,
                    Quantity = item.qty,
                    Price = item.price
                });

                //Constraint 3. Total Amount must be calculated in Service
                total += item.qty * item.price;
            }

            order.TotalAmount = total;

            await _orderRepo.AddAsync(order);
        }

        public async Task<Order?> UpdateStatusAsync(int orderId, string status)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return null;

            order.Status = status;
            await _orderRepo.UpdateAsync(order);
            return order;
        }
    }
}