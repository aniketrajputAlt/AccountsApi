using AccountsApi.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AccountsApi.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingAppDbContext _context;

        public AccountRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAccount(AccountInputModel input)
        {
            var useTransaction = _context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory";

            using (var transaction = useTransaction ? await _context.Database.BeginTransactionAsync() : null)
            {
                try
                {
                    // Check if the customer, account type, and branch exist
                    var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == input.CustomerID);
                    var accountTypeExists = await _context.AccountTypes.AnyAsync(a => a.TypeID == input.TypeID);
                    var branchExists = await _context.Branches.AnyAsync(b => b.BranchID == input.BranchID);

                    if (!customerExists)
                    {
                        throw new Exception("Customer with the provided ID does not exist.");
                    }

                    if (!accountTypeExists)
                    {
                        throw new Exception("Account type with the provided ID does not exist.");
                    }

                    if (!branchExists)
                    {
                        throw new Exception("Branch with the provided ID does not exist.");
                    }

                    // Set withdrawal and deposit quotas based on account type
                    int wdQuota = 0;
                    int dpQuota = 0;
                    if (input.TypeID == 1)
                    {
                        wdQuota = 10;
                        dpQuota = 5;
                    }
                    else if (input.TypeID == 2)
                    {
                        wdQuota = int.MaxValue; // Unlimited
                        dpQuota = 30;
                    }
                    else
                    {
                        throw new Exception("Invalid account type.");
                    }

                    // Check if the account balance meets the requirements based on account type
                    if ((input.TypeID == 1 && input.Balance < 1000) || (input.TypeID == 2 && input.Balance < 5000))
                    {
                        throw new Exception("Account balance is not enough.");
                    }

                    var account = new Account
                    {
                        Balance = input.Balance,
                        wd_Quota = wdQuota,
                        dp_Quota = dpQuota,
                        isActive = true,
                        CustomerID = input.CustomerID,
                        TypeID = input.TypeID,
                        BranchID = input.BranchID
                    };

                    _context.Accounts.Add(account);
                    await _context.SaveChangesAsync();

                    if (useTransaction)
                    {
                        await transaction.CommitAsync(); // Commit the transaction
                    }

                    return true;
                }
                catch (DbUpdateException dbEx)
                {
                    if (useTransaction)
                    {
                        await transaction.RollbackAsync(); // Rollback the transaction if an exception occurs
                    }
                    throw new Exception("Database update exception: " + dbEx.InnerException?.Message ?? dbEx.Message);
                }
                catch (Exception ex)
                {
                    if (useTransaction)
                    {
                        await transaction.RollbackAsync(); // Rollback the transaction if an exception occurs
                    }
                    throw new Exception("Exception: " + ex.InnerException?.Message ?? ex.Message); // Throw the caught exception to bubble it up
                }
            }
        }
        public async Task<Account> GetAccountById(long id)
        {
            try
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == id);
                if (account == null)
                {
                    // If the account is not found, return null
                    throw new Exception("Account with the provided ID does not exist.");
                }
                if (account.isActive == false)
                {
                    // If the account is not found, return null
                    throw new Exception("Account was deleted by the user");
                }




                return account;
            }
            catch (Exception ex)
            {
                // If any exception occurs, throw it to bubble it up
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteAccount(long id)
        {
            var useTransaction = _context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory";

            using (var transaction = useTransaction ? await _context.Database.BeginTransactionAsync() : null)
            {
                try
                {
                    var account = await _context.Accounts.FindAsync(id);
                    if (account == null)
                    {
                        throw new Exception("Account with the provided ID does not exist.");
                    }
                    if (account.isActive == false)
                    {
                        throw new Exception("Account was already deleted by the user");
                    }


                    // Set isActive to false
                    account.isActive = false;

                    // Mark the entity as modified
                    _context.Entry(account).State = EntityState.Modified;

                    // Save the changes
                    await _context.SaveChangesAsync();

                    if (useTransaction)
                    {
                        await transaction.CommitAsync(); // Commit the transaction
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    if (useTransaction)
                    {
                        await transaction.RollbackAsync(); // Rollback the transaction if an exception occurs
                    }
                    throw new Exception(ex.Message); // Throw the caught exception to bubble it up
                }
            }
        }

        public async Task<List<Account>> GetAccountsByCustomerId(int customerId)
        {
            return await _context.Accounts
                .Where(c => c.isActive && c.CustomerID == customerId).ToListAsync();
        }
    }
}
