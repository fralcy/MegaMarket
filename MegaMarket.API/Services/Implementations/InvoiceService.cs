using MegaMarket.API.DTOs.Invoice;
using MegaMarket.API.Services.Interfaces;
using MegaMarket.Data.Models;
using MegaMarket.Data.Repositories.Interfaces;

namespace MegaMarket.API.Services.Implementations
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _repository;
        public InvoiceService(IInvoiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Invoice>> GetAllInvoices()
        {
            var listInvoices = await _repository.GetAllInvoices();
            return listInvoices;
        }
        public async Task<InvoiceDto> SaveInvoice(InvoiceDto invoiceDto)
        {
            var invoiceDetails = new List<InvoiceDetail>();
            foreach (var detailDto in invoiceDto.InvoiceDetails)
            {
                var detail = new InvoiceDetail
                {
                    ProductId = detailDto.ProductId,
                    Quantity = detailDto.Quantity,
                    UnitPrice = detailDto.UnitPrice,
                    DiscountPerUnit = detailDto.DiscountPerUnit,
                    PromotionId = detailDto.PromotionId
                };
                invoiceDetails.Add(detail);
            }

            var invoice = new Invoice
            {
                UserId = 1,
                TotalBeforeDiscount = invoiceDto.TotalBeforeDiscount,
                TotalAmount = invoiceDto.TotalAmount,
                ReceivedAmount = invoiceDto.ReceivedAmount,
                ChangeAmount = invoiceDto.ChangeAmount,
                PromotionId = invoiceDto.PromotionId,
            };
            
            foreach (var detail in invoiceDetails)
            {
                invoice.InvoiceDetails.Add(detail);
            }

            var savedInvoice = await _repository.SaveInvoice(invoice);
            var result = new InvoiceDto
            {
                TotalBeforeDiscount = savedInvoice.TotalBeforeDiscount,
                TotalAmount = savedInvoice.TotalAmount,
                ReceivedAmount = savedInvoice.ReceivedAmount,
                ChangeAmount = savedInvoice.ChangeAmount,
                PromotionId = savedInvoice.PromotionId,
                InvoiceDetails = invoiceDetails.Select(d => new InvoiceDetailDto
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    DiscountPerUnit = d.DiscountPerUnit,
                    PromotionId = d.PromotionId
                }).ToList()
            };

            return result;
        }
    }
}
