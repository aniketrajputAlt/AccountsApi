using AccountsApi.Model;
using Microsoft.EntityFrameworkCore;

namespace AccountsApi.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingAppDbContext _context;
        public AccountRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        public async Task<Account> CreateNew(Account account)
        {
            throw new NotImplementedException();
        }

        public async Task<bool>DeleteAccount(long accountId)
        {
            throw new NotImplementedException();
        }

        public async Task<Account>GetDetails(long accountId)
        {
            throw new NotImplementedException();
        }
        public async Task<List<Account>> GetAccountsByCustomerId(int customerId)
        {
            return await _context.Accounts
                .Where(c => c.isActive && c.CustomerID == customerId).ToListAsync();
        }
    }
}
