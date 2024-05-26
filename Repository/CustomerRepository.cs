using AccountsApi.Model;
using Microsoft.EntityFrameworkCore;

namespace AccountsApi.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly BankingAppDbContext _context;


        public CustomerRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetActiveCustomerByIdAsync(int customerId)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsActive);
        }

        public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
        {
            return await _context.Customers
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<bool> DeactivateCustomerAsync(int customerId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsActive);

            if (customer == null)
                return false;

            customer.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customer.CustomerId && c.IsActive);

            if (existingCustomer == null)
                return null;

            // Update fields
            existingCustomer.FirstName = customer.FirstName ?? existingCustomer.FirstName;
            existingCustomer.LastName = customer.LastName ?? existingCustomer.LastName;
            existingCustomer.AddressLine1 = customer.AddressLine1 ?? existingCustomer.AddressLine1;
            existingCustomer.AddressLine2 = customer.AddressLine2 ?? existingCustomer.AddressLine2;
            existingCustomer.AddressLine3 = customer.AddressLine3 ?? existingCustomer.AddressLine3;
            existingCustomer.Pincode = customer.Pincode;
            existingCustomer.PhoneNumber = customer.PhoneNumber ?? existingCustomer.PhoneNumber;
            existingCustomer.EmailAddress = customer.EmailAddress ?? existingCustomer.EmailAddress;
            existingCustomer.DateOfBirth = customer.DateOfBirth;
            existingCustomer.City = customer.City ?? existingCustomer.City;
            existingCustomer.Country = customer.Country ?? existingCustomer.Country;

            await _context.SaveChangesAsync();
            return existingCustomer;
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            customer.IsActive = true; // Ensure the new customer is marked as active
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public Task<List<Account>> GetAccountsByCustomerId(int customerId)
        {
            return _context.Accounts
                .Where(c => c.isActive && c.CustomerID == customerId).ToListAsync();
        }
    }
}
