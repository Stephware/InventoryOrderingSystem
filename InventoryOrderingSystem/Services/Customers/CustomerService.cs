    using InventoryOrderingSystem.Helper;
    using InventoryOrderingSystem.Models;
    using InventoryOrderingSystem.Models.Database;
    using InventoryOrderingSystem.Repository.Customers;

    namespace InventoryOrderingSystem.Services.Customers
    {
        public class CustomerService : ICustomerService
        {
            private readonly ICustomerRepository _repo;

            public CustomerService(ICustomerRepository repo)
            {
                _repo = repo;
            }

            public async Task <bool> LoginCustomer(CustomerLoginModel model)
            {
                var userData = await _repo.GetUsernameAsync(model.Username);
                if (userData == null)
                {
                    return false;
                }

                var isPwMatch = SecurityHelper.VerifyPassword(model.Password, userData.PasswordHash);

                return isPwMatch;
            }

            public async Task RegisterUser(RegistrationModel model)
            {
                var existing = await _repo.GetUsernameAsync(model.Username);

                if (existing != null)
                    throw new InvalidOperationException("Username already exists");

                var user = new Customer
                {
                    Username = model.Username,
                    PasswordHash = SecurityHelper.HashPassword(model.Password),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    IsActive = true
                };

                await _repo.AddAsync(user);
            }

            public async Task<List<Customer>> GetAllAsync()
                => await _repo.GetAllAsync();

            public async Task AddAsync(Customer customer)
                => await _repo.AddAsync(customer);

            public async Task<Customer?> GetByIdAsync(int id)
            {
                return await _repo.GetByIdAsync(id);
            }

            public async Task UpdateAsync(Customer customer)
            {
                await _repo.UpdateAsync(customer);
            }

            public async Task <Customer?> GetByUsernameAsync (string username)
            {
                return await _repo.GetUsernameAsync (username);
            }
        }
    }