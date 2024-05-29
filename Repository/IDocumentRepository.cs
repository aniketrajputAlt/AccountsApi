using AccountsApi.Model;

namespace AccountsApi.Repository
{
    public interface IDocumentRepository
    {
       public Task InsertDocumentAsync(int customerId, byte[] document, int docType);
       //public Task UpdateDocumentAsync(int docId, byte[] document, int docType);
       public Task<IEnumerable<Document>> GetDocumentsByCustomerIdAsync(int customerId);
       //public Task<int?> GetDocumentIdByCustomerAndTypeAsync(int customerId, int docType);
    }
}
