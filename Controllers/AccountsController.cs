using AccountsApi.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;

namespace AccountsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        public AccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        [HttpGet("Customer/{id}")]
        public async Task<IActionResult> GetAccountsByCustomerId(int id)
        {
            try
            {
                var account = await _accountRepository.GetAccountsByCustomerId(id);
                if (account == null)
                {
                    return NotFound(new { message = "Accounts not found." });
                }
                return Ok(account);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving account data corresponding to the customer Id", error = ex.Message });
            }
        }
    }
}
