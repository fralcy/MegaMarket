using MegaMarket.API.DTOs.Invoice;
using MegaMarket.API.DTOs.Products;
using MegaMarket.API.DTOs.User;
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
        // Service methods for Invoice entity
        public async Task<List<InvoiceResDto>> GetAllInvoices()
        {
            var listInvoices = await _repository.GetAllInvoices();
            var result = listInvoices.Select(inv => new InvoiceResDto
            {
                InvoiceId = inv.InvoiceId,
                UserId = inv.UserId,
                TotalBeforeDiscount = inv.TotalBeforeDiscount,
                TotalAmount = inv.TotalAmount,
                ReceivedAmount = inv.ReceivedAmount,
                ChangeAmount = inv.ChangeAmount,
                PromotionId = inv.PromotionId,
                User = new UserResDto
                {
                    UserId = inv.User.UserId,
                    Username = inv.User.Username,
                    FullName = inv.User.FullName,
                    Email = inv.User.Email,
                    Role = inv.User.Role
                },
                InvoiceDetails = inv.InvoiceDetails.Select(d => new InvoiceDetailResDto
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    DiscountPerUnit = d.DiscountPerUnit,
                    PromotionId = d.PromotionId,
                    Product = new ProductDto
                    {
                        ProductId = d.Product.ProductId,
                        Name = d.Product.Name,
                        UnitPrice = d.Product.UnitPrice,
                    }
                }).ToList()
            }).ToList();

            return result;
        }
        public async Task<InvoiceResDto> SaveInvoice(InvoiceReqDto invoiceDto)
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
                UserId = 2,
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
            var result = new InvoiceResDto
            {
                UserId = savedInvoice.UserId,
                TotalBeforeDiscount = savedInvoice.TotalBeforeDiscount,
                TotalAmount = savedInvoice.TotalAmount,
                ReceivedAmount = savedInvoice.ReceivedAmount,
                ChangeAmount = savedInvoice.ChangeAmount,
                PromotionId = savedInvoice.PromotionId,
                User = new UserResDto
                {
                    UserId = savedInvoice.User.UserId,
                    Username = savedInvoice.User.Username,
                    FullName = savedInvoice.User.FullName,
                    Email = savedInvoice.User.Email,
                    Role = savedInvoice.User.Role
                },
                InvoiceDetails = savedInvoice.InvoiceDetails.Select(d => new InvoiceDetailResDto
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    DiscountPerUnit = d.DiscountPerUnit,
                    PromotionId = d.PromotionId,
                    Product = new ProductDto
                    {
                        ProductId = d.Product.ProductId,
                        Name = d.Product.Name,
                        UnitPrice = d.Product.UnitPrice,
                    }
                }).ToList()
            };

            return result;
        }
    }
}
