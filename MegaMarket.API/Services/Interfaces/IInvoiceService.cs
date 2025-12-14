using MegaMarket.API.DTOs.Invoice;
using MegaMarket.Data.Models;

namespace MegaMarket.API.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<List<Invoice>> GetAllInvoices();
        Task<InvoiceResDto> SaveInvoice(InvoiceReqDto invoiceDto);
    }
}
