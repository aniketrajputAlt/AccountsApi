using AccountsApi.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace AccountsApi.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BankingAppDbContext _context;


        public TransactionRepository(BankingAppDbContext context)
        {
            _context = context;
        }

      

        public async Task<bool> FundTransfer(long sourceAccountId, long destinationAccountId, decimal amount)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Check if the source account exists
                    var sourceAccount = await _context.Accounts.FindAsync(sourceAccountId);
                    if (sourceAccount == null)
                        throw new Exception("Source account does not exist.");

                    // Check if the destination account exists
                    var destinationAccount = await _context.Accounts.FindAsync(destinationAccountId);
                    if (destinationAccount == null)
                        throw new Exception("Destination account does not exist.");

                    // Check if the source account has sufficient balance
                    if (sourceAccount.TypeID==1 &&  sourceAccount.Balance-amount<1000)
                        throw new Exception("Insufficient balance in Savings account.");
                    if (sourceAccount.TypeID == 2 && sourceAccount.Balance - amount < 5000)
                        throw new Exception("Insufficient balance in Current account.");

                    // Check if source and destination accounts are the same
                    if (sourceAccountId == destinationAccountId)
                        throw new Exception("Source account same as Destination account.");

                    if(amount==0)
                        throw new Exception("Amount must be greater than zero.");

                    if (amount < 0)
                        throw new Exception("Amount must be greater than zero.");

                    // Calculate charges for Source account
                    decimal sourceWdCharges = CalculateCharges(sourceAccount.TypeID, sourceAccount.wd_Quota, amount, true);

                    // Calculate charges for Destination account
                    decimal destinationDpCharges = CalculateCharges(destinationAccount.TypeID, destinationAccount.dp_Quota, amount, false);

                    // Deduct the amount including transaction charges from the source account
                    sourceAccount.Balance -= amount + sourceWdCharges;

                    // Add the amount to the destination account including transaction charges
                    destinationAccount.Balance += amount - destinationDpCharges;

                    // Update quotas
                    if (sourceAccount.wd_Quota > 0)
                        sourceAccount.wd_Quota -= 1;

                    if (destinationAccount.dp_Quota > 0)
                        destinationAccount.dp_Quota -= 1;



                    // Log the transaction
                    var transactionLog = new Transaction
                    {
                        Amount = amount,
                        Time = DateTime.UtcNow,
                        Source_acc = sourceAccountId,
                        Dest_acc = destinationAccountId
                    };
                    _context.Transactions.Add(transactionLog);

                    // Save all changes to the database
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log the exception
                    throw new Exception(ex.Message);
                    return false;
                }
            }
        }

        private decimal CalculateCharges(int accountType, int quota, decimal amount, bool isWithdrawal)
        {
            decimal charges = 0;
            if (accountType == 1) // Savings
            {
                charges = quota > 0 ? 0 : 0.0001m * amount;
            }
            else if (accountType == 2) // Current
            {
                charges = quota > 0 ? 0 : 0.00025m * amount;
            }
            return charges;
        }

      
        public async Task<IEnumerable<Transaction>> TransactionsList(long accoundId)
        {
            long id = accoundId;
            return await _context.Transactions
                                  .Where(t => t.Source_acc == id || t.Dest_acc == id)
                                  .ToListAsync();
        }

       
    }
}

