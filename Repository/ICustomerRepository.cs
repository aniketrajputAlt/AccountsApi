using AccountsApi.Model;

namespace AccountsApi.Repository
{
    public interface ICustomerRepository
    {
        public Task<Customer> GetActiveCustomerByIdAsync(int customerId);
     //   public Task<IEnumerable<Customer>> GetActiveCustomersAsync();
     //  public  Task<bool> DeactivateCustomerAsync(int customerId);
       public  Task<Customer> UpdateCustomerAsync(Customer customer);
       public Task<Customer> CreateCustomerAsync(Customer customer);
    }
}
