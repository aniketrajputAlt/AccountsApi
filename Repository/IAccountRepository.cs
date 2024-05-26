using AccountsApi.Model;

namespace AccountsApi.Repository
{
    public interface IAccountRepository
    {
       public Task<Account>CreateNew(Account account);
        public Task<Account>GetDetails(long accountId);

        public Task<bool>DeleteAccount(long accountId);

        public Task<List<Account>> GetAccountsByCustomerId(int customerId);
    }
}
