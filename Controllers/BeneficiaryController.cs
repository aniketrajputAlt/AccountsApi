using AccountsApi.Model;
using AccountsApi.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeneficiaryController : ControllerBase
    {
        private readonly IBeneficiaryRepository _beneficiaryRepository;
        public BeneficiaryController(IBeneficiaryRepository beneficiaryRepository)
        {
            _beneficiaryRepository = beneficiaryRepository;
        }

        [HttpGet("Account/{accountId}")]
        public async Task<ActionResult<IEnumerable<Beneficiary>>> GetBeneficiariesByAccountId(long accountId)
        {
            try
            {
                var beneficiaries = await _beneficiaryRepository.ListBeneficiary(accountId);
                return Ok(beneficiaries);
            }
            catch (ArgumentException ex)
            {
                // Log the exception or handle it appropriately
                return BadRequest(ex.Message); // Return error message to client
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while retrieving beneficiaries");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Beneficiary>> PostBeneficiary(BeneficiaryInputModel beneficiary)
        {
            try
            {
                var addedBeneficiary = await _beneficiaryRepository.Addbeneficiary(beneficiary);
                return CreatedAtAction(nameof(GetBeneficiariesByAccountId), new { accountId = beneficiary.AccountId }, addedBeneficiary);
            }
            catch (ArgumentException ex)
            {
                // Log the exception or handle it appropriately
                return BadRequest(ex.Message); // Return error message to client
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBeneficiary(long id)
        {
            try
            {
                var result = await _beneficiaryRepository.DeleteBenficiary(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                // Log the exception or handle it appropriately
                return BadRequest(ex.Message); // Return error message to client
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while deleting beneficiary");
            }
        }
    }
}
