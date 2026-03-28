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
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                });
        }

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
            {
                TempData["Error"] = "Please fill in all required fields.";
                return View(model);
            }

            try
            {
                string adminUsername = "admin";
                string adminPassword = "123456";

                if (model.Username == adminUsername && model.Password == adminPassword)
                {
                    await SignInUser(adminUsername, "Admin");
                    TempData["LoginSuccess"] = "Welcome Admin!";
                    return RedirectToAction("Index", "Order");
                }

                var customer = await _customerService.GetByUsernameAsync(model.Username);

                if (customer == null)
                    throw new Exception("Username does not exist.");

                if (!SecurityHelper.VerifyPassword(model.Password, customer.PasswordHash))
                    throw new Exception("Incorrect password.");

                if (!customer.IsActive)
                    throw new Exception("Account is inactive.");

                await SignInUser(customer.Username, "Customer");

                TempData["LoginSuccess"] = $"Welcome, {customer.Username}!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegistrationModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage;

                return View(model);
            }

            try
            {
                await _customerService.RegisterUser(model);

                TempData["Success"] = "Registration successful!";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["LogoutSuccess"] = "You have been logged out.";
            return RedirectToAction("Login");
        }
    }
}