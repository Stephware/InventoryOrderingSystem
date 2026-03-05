using System;
using System.Threading.Tasks;
using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repository.Customers;
using InventoryOrderingSystem.Repository.Orders;
using InventoryOrderingSystem.Repository.Products;
using InventoryOrderingSystem.Services.Orders;
using Moq;
using Xunit;

namespace InventoryOrderingSystemTests
{
    public class OrderServiceTests
    {
        private readonly Mock<ICustomerRepository> _mockCustomerRepo;
        private readonly Mock<IProductRepository> _mockProductRepo;
        private readonly Mock<IOrderRepository> _mockOrderRepo;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockCustomerRepo = new Mock<ICustomerRepository>();
            _mockProductRepo = new Mock<IProductRepository>();
            _mockOrderRepo = new Mock<IOrderRepository>();
            _orderService = new OrderService(
                _mockCustomerRepo.Object,
                _mockProductRepo.Object,
                _mockOrderRepo.Object);
        }

        [Fact]
        public async Task PlaceOrderAsync_Rule2_InactiveCustomer_ThrowsException()
        {
            var inactive = new Customer { CustomerId = 1, IsActive = false };
            _mockCustomerRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(inactive);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _orderService.PlaceOrderAsync(1, 100, 1));

            Assert.Equal("Customer is not active.", exception.Message);
        }

        [Fact]
        public async Task PlaceOrderAsync_Rule4_SuccessfulOrder_WillReduceProductStock()
        {
            var active = new Customer { CustomerId = 1, IsActive = true };
            var product = new Product { ProductId = 100, Stock = 10, Price = 15.00m };

            _mockCustomerRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(active);
            _mockProductRepo.Setup(repo => repo.GetByIdAsync(100)).ReturnsAsync(product);

            await _orderService.PlaceOrderAsync(1, 100, 5);

            Assert.Equal(5, product.Stock);
            _mockProductRepo.Verify(repo => repo.UpdateAsync(product), Times.Once);
        }

        [Fact]
        public async Task PlaceOrderAsync_Rule5_StockIsZero_ThrowsException()
        {
            var active = new Customer { CustomerId = 1, IsActive = true };
            var empty = new Product { ProductId = 100, Stock = 0, Price = 10.00m};

            _mockCustomerRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(active);
            _mockProductRepo.Setup(repo => repo.GetByIdAsync(100)).ReturnsAsync(empty);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _orderService.PlaceOrderAsync(1, 100, 5));

            Assert.Equal("Insufficient product stock.", exception.Message);
        }

        [Fact]
        public async Task PlaceOrderAsync_Rule1_CustomerDoesNotExist_ThrowsException()
        {
            _mockCustomerRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Customer?)null);
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _orderService.PlaceOrderAsync(1, 100, 1));

            Assert.Equal("Customer does not exist.", exception.Message);
        }

        [Fact]
        public async Task PlaceOrderAsync_Rule3_CalculatesTotalAmount()
        {
            var active = new Customer { CustomerId = 3, IsActive = true };
            var product = new Product { ProductId = 57, Stock = 15, Price = 35.00m };

            int quantity = 3;

            _mockCustomerRepo.Setup(repo => repo.GetByIdAsync(3)).ReturnsAsync(active);
            _mockProductRepo.Setup(repo => repo.GetByIdAsync(57)).ReturnsAsync(product);

            var order = await _orderService.PlaceOrderAsync(3, 57, quantity);

            Assert.Equal(105.00m, order.TotalAmount);
            _mockOrderRepo.Verify(repo=>repo.AddAsync(It.IsAny<Order>()), Times.Once());
        }

        [Fact]
        public async Task PlaceOrderAsync_ProductDoesNotExist_ThrowsException()
        {
            var active = new Customer { CustomerId = 3, IsActive = true };

            _mockCustomerRepo.Setup(repo => repo.GetByIdAsync(3)).ReturnsAsync(active);
            _mockProductRepo.Setup(repo => repo.GetByIdAsync(57)).ReturnsAsync((Product)null);

            var exception = await Assert.ThrowsAsync<InvalidOperationException > (
                () => _orderService.PlaceOrderAsync(3, 157, 1));

            Assert.Equal("Product does not exist.", exception.Message);
        }

        [Fact]
        public async Task PlaceOrderAsync_StockIsMoreThanZero_ButRequestsGreaterThanTheStock()
        {
            var active = new Customer { CustomerId = 1, IsActive = true };
            var stock = new Product { ProductId = 4, Stock = 56, Price = 20.00m };

            _mockCustomerRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(active);
            _mockProductRepo.Setup(repo => repo.GetByIdAsync(4)).ReturnsAsync(stock);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _orderService.PlaceOrderAsync(1, 4, 57));

            Assert.Equal("Insufficient product stock.", exception.Message);
        }

    }
}
