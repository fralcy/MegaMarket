using MegaMarket.API.DTOs.Invoice;
using MegaMarket.API.Services.Interfaces;
using MegaMarket.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MegaMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _service;
        public InvoiceController(IInvoiceService service)
        {
            _service = service;
        }

        // POST: InvoiceController/Create
        [HttpPost]
        public async Task<ActionResult> SaveInvoice([FromBody]InvoiceRequestDto invoice)
        {
            try
            {
                var savedInvoice = await _service.SaveInvoice(invoice);
                return Ok(savedInvoice);
            }
            catch
            {
                return BadRequest(ModelState);
            }
        }
    }
}
