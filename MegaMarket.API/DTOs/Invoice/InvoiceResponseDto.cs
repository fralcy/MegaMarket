using MegaMarket.Data.Models;

namespace MegaMarket.API.DTOs.Invoice
{
    public class InvoiceResponseDto
    {
        public int InvoiceId { get; set; }
        public int UserId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal TotalBeforeDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = "cash";
        public decimal ReceivedAmount { get; set; }
        public decimal ChangeAmount { get; set; }
        public int? PromotionId { get; set; }
        public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    }
}
