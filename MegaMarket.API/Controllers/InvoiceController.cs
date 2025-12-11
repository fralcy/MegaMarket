using MegaMarket.Data.Models;
using MegaMarket.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MegaMarket.API.Controllers
{
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
