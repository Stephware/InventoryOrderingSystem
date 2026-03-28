using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Services.Customers;
using InventoryOrderingSystem.Services.Orders;
using InventoryOrderingSystem.Services.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryOrderingSystem.Controllers
{
    public class OrderSubmitForm
    {
        public int CustomerId { get; set; }
        public List<OrderItemForm> Items { get; set; } = new List<OrderItemForm>();
    }

    public class OrderItemForm
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    [Authorize(Roles = "Admin, Customer")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;

        public OrderController(IOrderService orderService, ICustomerService customerService, IProductService productService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _productService = productService;
        }

        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Index(string search, int page = 1)
        {
            int pageSize = 10;
            IEnumerable<Order> orders;

            if (User.IsInRole("Admin"))
            {
                orders = await _orderService.GetAllAsync();
            }
            else
            {
                var username = User.Identity.Name;
                var customer = await _customerService.GetByUsernameAsync(username);

                if (customer == null)
                    return Unauthorized();

                var allOrders = await _orderService.GetAllAsync();
                orders = allOrders.Where(o => o.CustomerId == customer.CustomerId);
            }

            if (!string.IsNullOrEmpty(search))
            {
                orders = orders.Where(o =>
                    (o.Status != null && o.Status.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    o.OrderId.ToString().Contains(search)
                ).ToList();
            }

            int totalItems = orders.Count();

            var paginatedOrders = orders
                .OrderByDescending(o => o.DateCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalItems = totalItems;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;

            return View(paginatedOrders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [Authorize(Roles = "Admin,Customer")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.Customers = await _customerService.GetAllAsync();
            }

            ViewBag.Products = await _productService.GetAllProductsAsync();
            return View();
        }

        [Authorize(Roles = "Admin,Customer")]
        [HttpPost]
        public async Task<IActionResult> Create(OrderSubmitForm form)
        {
            if (form.Items == null || !form.Items.Any(i => i.Quantity > 0))
            {
                TempData["Error"] = "Select at least one product.";
                return RedirectToAction("Create");
            }

            
            if (User.IsInRole("Customer"))
            {
                var username = User.Identity.Name;
                var customer = await _customerService.GetByUsernameAsync(username);

                if (customer == null)
                {
                    TempData["Error"] = "Customer not found.";
                    return RedirectToAction("Create");
                }

                form.CustomerId = customer.CustomerId;
            }

            var items = form.Items
                .Where(i => i.Quantity > 0)
                .Select(i => (i.ProductId, i.Quantity))
                .ToList();

            var newOrderId = await _orderService.CreateOrderAsync(form.CustomerId, items);

            try
            {
                await _orderService.CreateOrderAsync(form.CustomerId, items);
                TempData["Success"] = "Order created successfully!";
                return RedirectToAction("Details", new {id=newOrderId});
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Create");
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkCompleted(int id)
        {
            await _orderService.UpdateStatusAsync(id, "Completed");
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            ViewBag.Products = await _productService.GetAllProductsAsync();

            return View(order);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int orderId, List<OrderItemForm> items)
        {
            try
            {
                var formattedItems = items
                    .Where(i => i.Quantity > 0)
                    .Select(i => (i.ProductId, i.Quantity))
                    .ToList();

                await _orderService.UpdateOrderItemsAsync(orderId, formattedItems);

                TempData["Success"] = "Order updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Edit", new { id = orderId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                await _orderService.CancelOrderAsync(id);
                TempData["Success"] = "Order cancelled.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}