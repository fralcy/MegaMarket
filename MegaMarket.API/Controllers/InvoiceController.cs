using MegaMarket.Data.Models;
using MegaMarket.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MegaMarket.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private IInvoiceRepository _repository;
        public InvoiceController(IInvoiceRepository repository)
        {
            _repository = repository;
        }

        // POST: InvoiceController/Create
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Cashier")]
        public async Task<ActionResult<Invoice>> SaveInvoice([FromBody]Invoice invoice)
        {
            try
            {
                await _repository.SaveInvoice(invoice);
                return Ok(invoice);
            }
            catch
            {
                return BadRequest(ModelState);
            }
        }
    }
}
