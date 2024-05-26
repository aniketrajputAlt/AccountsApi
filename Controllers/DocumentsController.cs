using AccountsApi.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentRepository _documentsRepository;

        public DocumentsController(IDocumentRepository documentsRepository)
        {
            _documentsRepository = documentsRepository;
        }

        [HttpGet("{id}")]
        [EnableCors]
        public async Task<IActionResult> GetCustomerDocuments(int id)
        {
            try
            {
                var documents = await _documentsRepository.GetDocumentsByCustomerIdAsync(id);
                if (documents == null || !documents.Any())
                {
                    return NotFound(new { message = "Customer not found or no documents available." });
                }

                var result = documents.Select(d => new
                {
                    d.DocId,
                    d.CustomerId,
                    d.DocTypeId,
                    Documents = Convert.ToBase64String(d.Documents),
                    d.IsActive
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception details here if necessary
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving customer documents", error = ex.Message });
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument(int customerId, IFormFile file, int docType)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Invalid file." });
                }

                byte[] documentData = ConvertToBytes(file);

                await _documentsRepository.InsertDocumentAsync(customerId, documentData, docType);

                return Ok(new { message = "Document uploaded successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception details here if necessary
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error uploading document", error = ex.Message });
            }
        }

        private byte[] ConvertToBytes(IFormFile file)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
