using AccountsApi.Model;
using Microsoft.EntityFrameworkCore;

namespace AccountsApi.Repository
{
    
        public class BeneficiaryRepository : IBeneficiaryRepository
        {
            private readonly BankingAppDbContext _context;

            public BeneficiaryRepository(BankingAppDbContext context)
            {
                _context = context;
            }

            public async Task<Beneficiary> Addbeneficiary(BeneficiaryInputModel beneficiaryInput)
            {
                try
                {
                    if (beneficiaryInput == null)
                    {
                        throw new ArgumentNullException(nameof(beneficiaryInput), "Beneficiary input model cannot be null.");
                    }
                    if (string.IsNullOrWhiteSpace(beneficiaryInput.BenefName))
                    {
                        throw new ArgumentException("Beneficiary name is required.", nameof(beneficiaryInput.BenefName));
                    }
                    if (string.IsNullOrWhiteSpace(beneficiaryInput.BenefName))
                    {
                        throw new ArgumentException("Beneficiary name is required.", nameof(beneficiaryInput.BenefName));
                    }
                    if (beneficiaryInput.BenefAccount.ToString().Length != 13 || !beneficiaryInput.BenefAccount.ToString().All(char.IsDigit))
                    {
                        throw new ArgumentException("Beneficiary account number must be a 13-digit number.", nameof(beneficiaryInput.BenefAccount));
                    }
                    if (beneficiaryInput.AccountId.ToString().Length != 13 || !beneficiaryInput.AccountId.ToString().All(char.IsDigit))
                    {
                        throw new ArgumentException("Account ID must be a 13-digit number.", nameof(beneficiaryInput.AccountId));
                    }
                    // Check if BenefAccount exists in the Accounts table
                    var accountExists = await _context.Accounts.AnyAsync(a => a.AccountId == beneficiaryInput.BenefAccount);
                    if (!accountExists)
                    {
                        throw new ArgumentException("Beneficiary Account does not exist in Accounts.", nameof(beneficiaryInput.BenefAccount));
                    }

                    // Check if AccountId exists in the Accounts table
                    var mainAccountExists = await _context.Accounts.AnyAsync(a => a.AccountId == beneficiaryInput.AccountId);
                    if (!mainAccountExists)
                    {
                        throw new ArgumentException("Main account does not exist in Accounts.", nameof(beneficiaryInput.AccountId));
                    }



                    var beneficiary = new Beneficiary
                    {
                        BenefName = beneficiaryInput.BenefName,
                        BenefAccount = beneficiaryInput.BenefAccount,
                        BenefIFSC = beneficiaryInput.BenefIFSC,
                        AccountId = beneficiaryInput.AccountId,
                        IsActive = beneficiaryInput.IsActive
                    };
                    await _context.Beneficiaries.AddAsync(beneficiary);
                    await _context.SaveChangesAsync();
                    return beneficiary;
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error occurred while adding beneficiary");
                }
            }

    

        public async Task<bool> DeleteBenficiary(long beneficiaryId)
            {
                try
                {
                    var beneficiary = await _context.Beneficiaries.FirstOrDefaultAsync(b => b.BenefAccount == beneficiaryId && b.IsActive == true);
                    if (beneficiary == null)
                    {
                        return false;
                    }

                    beneficiary.IsActive = false;
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error occurred while deleting beneficiary", ex);
                }
            }

            public async Task<IEnumerable<Beneficiary>> ListBeneficiary(long accountId)
            {
                try
                {
                    return await _context.Beneficiaries
                        .Where(b => b.AccountId == accountId && b.IsActive == true)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it appropriately
                    throw new Exception("Error occurred while listing beneficiaries", ex);
                }
            }
        
    }
}
