using AccountsApi.Model;

namespace AccountsApi.Repository
{
    public interface IBeneficiaryRepository
    {
       public  Task<Beneficiary> Addbeneficiary(BeneficiaryInputModel beneficiaryInput);

       public Task<IEnumerable<Beneficiary>> ListBeneficiary(long accoundId);

       public Task<bool>DeleteBenficiary(long accountId,long beneficiaryId);
    }
}
