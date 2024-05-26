using AccountsApi.Model;
using Microsoft.EntityFrameworkCore;

namespace AccountsApi.Repository
{
    public class DocumentRepository:IDocumentRepository
    {
        private readonly BankingAppDbContext _context;

        public DocumentRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        public async Task InsertDocumentAsync(int customerId, byte[] document, int docType)
        {
            var newDocument = new Document
            {
                CustomerId = customerId,
                DocTypeId = docType,
                Documents = document,
                IsActive = true
            };
            _context.Documents.Add(newDocument);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDocumentAsync(int docId, byte[] document, int docType)
        {
            var existingDocument = await _context.Documents.FindAsync(docId);
            if (existingDocument != null)
            {
                existingDocument.Documents = document;
                existingDocument.DocTypeId = docType;
                existingDocument.IsActive = true; // Ensure the document is marked as active
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Document>> GetDocumentsByCustomerIdAsync(int customerId)
        {

            return await _context.Documents
                .Where(d => d.CustomerId == customerId && d.IsActive)
                .ToListAsync();

        }





        public async Task<int?> GetDocumentIdByCustomerAndTypeAsync(int customerId, int docType)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.CustomerId == customerId && d.DocTypeId == docType && d.IsActive);
            return document?.DocId;
        }

       
    }
}
