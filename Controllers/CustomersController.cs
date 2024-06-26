﻿using AccountsApi.Model;
using AccountsApi.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet("GetActiveCustomer/{id}")]
        //[Authorize(Policy = "CustomerDataAccess")]

        public async Task<IActionResult> GetActiveCustomer(int id)
        {
            
                var customer = await _customerRepository.GetActiveCustomerByIdAsync(id);
                if (customer == null)
                {
                    return NotFound(new { message = "Customer not found." });
                }
                return Ok(customer);
            
           
        }


        [HttpPut("updateCustomer/{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest(new { message = "Mismatched customer ID." });
            }

         
                var updatedCustomer = await _customerRepository.UpdateCustomerAsync(customer);
                if (updatedCustomer == null)
                {
                    return NotFound(new { message = "Customer not found or inactive." });
                }
                return Ok(updatedCustomer);
           
        }

        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
        {
            if (customer == null)
            {
                return BadRequest(new { message = "Customer data cannot be null." });
            }

            var createdCustomer = await _customerRepository.CreateCustomerAsync(customer);
            if (createdCustomer == null)
            {
                return BadRequest(new { message = "Invalid customer data. Please check the provided information." });
            }

            return CreatedAtAction(nameof(GetActiveCustomer), new { id = createdCustomer.CustomerId }, createdCustomer);
        }

    }
}
