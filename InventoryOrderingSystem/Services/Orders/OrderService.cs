using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repository.Customers;
using InventoryOrderingSystem.Repository.Orders;
using InventoryOrderingSystem.Repository.Products;
using Microsoft.EntityFrameworkCore;

namespace InventoryOrderingSystem.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;

        public OrderService(
            ICustomerRepository customerRepo,
            IProductRepository productRepo,
            IOrderRepository orderRepo)
        {   
            _customerRepo = customerRepo;
            _productRepo = productRepo;
            _orderRepo = orderRepo;
        }

        public async Task<Order> PlaceOrderAsync(int customerId, int productId, int quantity)
        {
            var customer = await _customerRepo.GetByIdAsync(customerId);
            if (customer == null)
                throw new InvalidOperationException("Customer does not exist.");

            if (!customer.IsActive)
                throw new InvalidOperationException("Customer is not active.");

            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null)
                throw new InvalidOperationException("Product does not exist.");

            if (product.Stock == 0 || product.Stock < quantity)
                throw new InvalidOperationException("Insufficient product stock.");

            decimal totalAmount = product.Price * quantity;

            product.Stock -= quantity;
            try
            {
                await _productRepo.UpdateAsync(product);
            }
            catch (DbUpdateConcurrencyException)
            {
                // This catches the race condition!
                throw new InvalidOperationException("Someone else just purchased this item. Please try again to check new stock levels.");
            }

            var order = new Order
            {
                CustomerId = customerId,
                ProductId = productId,
                Quantity = quantity,
                TotalAmount = totalAmount
            };

            await _orderRepo.AddAsync(order);

            return order;
        }
    }
}