using MegaMarket.API.DTOs.Customers;
using MegaMarket.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace MegaMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IPointTransactionService _pointTransactionService;
        public CustomersController(ICustomerService customerService, IPointTransactionService pointTransactionService)
        {
            _customerService = customerService;
            _pointTransactionService = pointTransactionService;
        }

        //api/customers : get all customers
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            // get all customers
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        //api/customers/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerById(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        //api/customers : create a customer
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // create a customer
            var response = await _customerService.CreateCustomerAsync(requestDto);
            if (response == null)
            {
                return BadRequest("Can't create customer. The phone may already exist.");
            }
            return CreatedAtAction(nameof(GetCustomerById), new { id = response.CustomerId }, response);
        }

        //api/customers/{id} : update customer
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _customerService.UpdateCustomerAsync(id, requestDto);
            if (response == null)
            {
                return BadRequest("Can't update customer. The phone may already exist.");
            }
            return Ok(response);
        }

        //api/customers/{id} : delete cutomer
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var response = await _customerService.DeleteCustomerAsync(id);
                if (response)
                {
                    return Ok("Delete successfully!");
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                // display status error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        //api/customers/search
        [HttpGet("search")]
        public async Task<IActionResult> FilterCustomers([FromQuery] FilterCustomerRequestDto filter)
        {
            var results = await _customerService.FilterCustomersAsync(filter);

            if (!results.Any())
            {
                return NotFound("No suitable customers were found.");
            }

            return Ok(results);
        }

        // GET: api/customers/{id}/points
        [HttpGet("{id}/Points")]
        public async Task<IActionResult> GetCustomerPointHistory(int id)
        {
            var history = await _pointTransactionService.GetPointHistoryAsync(id);

            if (!history.Any())
                return NotFound($"Customer {id} has no point history.");

            return Ok(history);
        }


    }
}
