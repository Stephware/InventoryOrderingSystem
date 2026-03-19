using InventoryOrderingSystem.Services.Orders;
using Microsoft.AspNetCore.Mvc;

namespace InventoryOrderingSystem.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: /Order/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Order/Create
        [HttpPost]
        public async Task<IActionResult> Create(int customerId, int productId, int quantity)
        {
            try
            {
                var order = await _orderService.PlaceOrderAsync(customerId, productId, quantity);
                TempData["SuccessMessage"] = $"Order #{order.OrderId} placed successfully for ${order.TotalAmount}!";
                return RedirectToAction("Create");
            }
            catch (InvalidOperationException ex)
            {
                // This catches the custom exceptions you throw in your OrderService
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred.";
                return View();
            }
        }
    }
}