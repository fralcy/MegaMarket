using MegaMarket.API.DTOs.Invoice;
using MegaMarket.API.Services.Interfaces;
using MegaMarket.Data.Models;
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
        private readonly IInvoiceService _service;
        public InvoiceController(IInvoiceService service)
        {
            _service = service;
        }
        // Controller methods for Invoice entity
        [HttpGet]
        public async Task<ActionResult> GetAllInvoices()
        {
            try
            {
                var listInvoices = await _service.GetAllInvoices();
                return Ok(listInvoices);
            } catch
            {
                return BadRequest(ModelState);
            }
        }
        // POST: InvoiceController/Create
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Cashier")]
        public async Task<ActionResult> SaveInvoice([FromBody]InvoiceReqDto invoice)
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
