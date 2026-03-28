using InventoryOrderingSystem.Services.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class CustomerController : Controller
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task<IActionResult> Index(string search, int page = 1)
    {
        int pageSize = 10;

        var customers = await _customerService.GetAllAsync();

        if (!string.IsNullOrEmpty(search))
        {
            customers = customers.Where(c =>
                (c.Username != null && c.Username.Contains(search, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        int totalItems = customers.Count();

        var paginatedCustomers = customers
            .OrderByDescending(c => c.CustomerId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.TotalItems = totalItems;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.Search = search;

        return View(paginatedCustomers);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null) return NotFound();

        customer.IsActive = !customer.IsActive;
        await _customerService.UpdateAsync(customer);

        return RedirectToAction("Index");
    }
}