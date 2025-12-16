using MegaMarket.API.DTOs.Products;
using MegaMarket.Data.Models;

namespace MegaMarket.API.DTOs.Invoice
{
    public class InvoiceDetailResDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPerUnit { get; set; } = 0;
        public int? PromotionId { get; set; }
        public ProductDto Product { get; set; } = null!;
    }
}
