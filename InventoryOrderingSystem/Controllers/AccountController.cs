using InventoryOrderingSystem.Helper;
using InventoryOrderingSystem.Models;
using InventoryOrderingSystem.Services.Customers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventoryOrderingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ICustomerService _customerService;

        public AccountController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        private async Task SignInUser(string username, string role)
        {
            var claims = new List<Claim>
          {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
          };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                });
        }

        // --- ADMIN LOGIN ---
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Order");

            return View(new AdminLoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string adminUsername = "admin";
            string adminPassword = "admin123"; // you can change this

            if (model.Username == adminUsername && model.Password == adminPassword)
            {
                await SignInUser(adminUsername, "Admin");
                return RedirectToAction("Index", "Order");
            }

            var customer = await _customerService.GetByUsernameAsync(model.Username);

            if (customer != null &&
                SecurityHelper.VerifyPassword(model.Password, customer.PasswordHash))
            {
                if (!customer.IsActive)
                {
                    ModelState.AddModelError("", "Customer account is inactive.");
                    return View(model);
                }

                await SignInUser(customer.Username, "Customer");
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View(model);
        }



        // --- REGISTRATION ---
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegistrationModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Awaits your service logic to create the customer
                    await _customerService.RegisterUser(model);

                    ViewBag.SuccessMessage = "Registration successful!";
                    ModelState.Clear();
                    return View(new RegistrationModel()); // Reset the form
                }
                catch (Exception ex)
                {
                    // If your service throws an exception (e.g., "Username taken")
                    ViewBag.ErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}