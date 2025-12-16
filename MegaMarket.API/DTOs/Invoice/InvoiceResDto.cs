using MegaMarket.API.DTOs.Customers;
using MegaMarket.API.DTOs.Promotion;
using MegaMarket.API.DTOs.User;
using MegaMarket.Data.Models;

namespace MegaMarket.API.DTOs.Invoice
{
    public class InvoiceResDto
    {
        public int InvoiceId { get; set; }
        public int UserId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal TotalBeforeDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = "cash"; // cash / bank_transfer / card
        public decimal ReceivedAmount { get; set; }
        public decimal ChangeAmount { get; set; }
        public int? PromotionId { get; set; }
        public string Status { get; set; } = "Pending"; // Paid / Pending
        public UserResDto User { get; set; } = null!;
        public CustomerResponseDto? Customer { get; set; }
        public PromotionResDto? Promotion { get; set; }
        public ICollection<InvoiceDetailResDto> InvoiceDetails { get; set; } = new List<InvoiceDetailResDto>();
    }
}
