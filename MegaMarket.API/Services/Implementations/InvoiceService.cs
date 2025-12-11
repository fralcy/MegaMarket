using MegaMarket.API.DTOs.Invoice;
using MegaMarket.API.Services.Interfaces;
using MegaMarket.Data.Models;
using MegaMarket.Data.Repositories;

namespace MegaMarket.API.Services.Implementations
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _repository;
        public InvoiceService(IInvoiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<Invoice> SaveInvoice(InvoiceRequestDto invoiceRequestDto)
        {
            var invoice = new Invoice
            {
                TotalBeforeDiscount = invoiceRequestDto.TotalBeforeDiscount,
                TotalAmount = invoiceRequestDto.TotalAmount,
                ReceivedAmount = invoiceRequestDto.ReceivedAmount,
                ChangeAmount = invoiceRequestDto.ChangeAmount,
                PromotionId = invoiceRequestDto.PromotionId,
            };
            var savedInvoice = await _repository.SaveInvoice(invoice);

            return savedInvoice;
        }
    }
}
