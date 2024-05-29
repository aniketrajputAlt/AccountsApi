using AccountsApi.Model;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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
            return await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsActive);
        }


        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            if (!IsValidCustomer(customer))
            {
                return null; // Return null if validation fails
            }

            customer.IsActive = true; // Ensure the new customer is marked as active
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        private bool IsValidCustomer(Customer customer)
        {
            // Check for required and correctly formatted fields
            return customer != null
                && !string.IsNullOrEmpty(customer.FirstName)
                && !string.IsNullOrEmpty(customer.LastName)
                && !string.IsNullOrEmpty(customer.AddressLine1)
                && IsValidPhoneNumber(customer.PhoneNumber)
                && IsValidEmail(customer.EmailAddress)
                && customer.DateOfBirth != default
                && !string.IsNullOrEmpty(customer.City)
                && !string.IsNullOrEmpty(customer.Country)
                && IsValidPincode(customer.Pincode);
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return phoneNumber != null && Regex.IsMatch(phoneNumber, @"^\d{10}$");
        }

        private bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
        }

        private bool IsValidPincode(int pincode)
        {
            return pincode.ToString().Length == 6;
        }


        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customer.CustomerId && c.IsActive);

            if (existingCustomer == null)
                return null;  // Returns null if no active customer found

            // Validates and updates fields only if they pass the validation checks
            existingCustomer.FirstName = ValidateField(customer.FirstName, existingCustomer.FirstName);
            existingCustomer.LastName = ValidateField(customer.LastName, existingCustomer.LastName);
            existingCustomer.AddressLine1 = ValidateField(customer.AddressLine1, existingCustomer.AddressLine1);
            existingCustomer.AddressLine2 = ValidateField(customer.AddressLine2, existingCustomer.AddressLine2);
            existingCustomer.AddressLine3 = ValidateField(customer.AddressLine3, existingCustomer.AddressLine3);
            existingCustomer.PhoneNumber = ValidatePhoneNumber(customer.PhoneNumber, existingCustomer.PhoneNumber);
            existingCustomer.EmailAddress = ValidateEmail(customer.EmailAddress, existingCustomer.EmailAddress);
            existingCustomer.City = ValidateField(customer.City, existingCustomer.City);
            existingCustomer.Country = ValidateField(customer.Country, existingCustomer.Country);

            // Special checks for non-string fields
            if (IsValidPincode(customer.Pincode)) existingCustomer.Pincode = customer.Pincode;
            if (customer.DateOfBirth != default) existingCustomer.DateOfBirth = customer.DateOfBirth;

            await _context.SaveChangesAsync();
            return existingCustomer;
        }


        private string ValidateField(string input, string currentValue)
        {
            return !string.IsNullOrWhiteSpace(input) ? input : currentValue;
        }

        private string ValidatePhoneNumber(string input, string currentValue)
        {
            // Assume phone number must be exactly 10 digits long
            return !string.IsNullOrWhiteSpace(input) && input.Length == 10 && input.All(char.IsDigit) ? input : currentValue;
        }

        private string ValidateEmail(string input, string currentValue)
        {
            // Simple email validation
            return !string.IsNullOrWhiteSpace(input) && Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") ? input : currentValue;
        }



    }
}
