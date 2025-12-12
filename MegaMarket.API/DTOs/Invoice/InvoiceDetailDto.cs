namespace MegaMarket.API.DTOs.Invoice
{
    public class InvoiceDetailDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPerUnit { get; set; } = 0;
        public int? PromotionId { get; set; }
    }
}
