using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repository.Customers;
using InventoryOrderingSystem.Repository.Orders;
using InventoryOrderingSystem.Repository.Products;
using InventoryOrderingSystem.Services.Customers;
using InventoryOrderingSystem.Services.Orders;
using InventoryOrderingSystem.Services.Products;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add MVC Services
builder.Services.AddControllersWithViews();

// 2. Configure Entity Framework Core
builder.Services.AddDbContext<InventoryOrderingSystemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Register Repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// 4. Register Services
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Order}/{action=Create}/{id?}"); // Defaulting to Order/Create for testing

app.Run();