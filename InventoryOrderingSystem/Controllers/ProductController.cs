using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Services.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryOrderingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Product());
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            try
            {
                await _productService.AddAsync(product);
                TempData["Success"] = "Product added successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(product);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStock(int id, int stock)
        {
            await _productService.UpdateStockAsync(id, stock);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> UpdatePrice(int id, decimal price)
        {
            if (price < 0)
            {
                ModelState.AddModelError("", "Price cannot be negative.");

                var products = await _productService.GetAllProductsAsync();
                return View("Index", products);
            }

            await _productService.UpdatePriceAsync(id, price);

            TempData["Success"] = "Price updated.";
            return RedirectToAction("Index");
        }
    }
}