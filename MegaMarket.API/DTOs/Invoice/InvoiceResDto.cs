using MegaMarket.API.DTOs.User;
using MegaMarket.Data.Models;

namespace MegaMarket.API.DTOs.Invoice
{
    public class InvoiceResDto
    {
        public int UserId { get; set; }
        public decimal TotalBeforeDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = "cash"; // cash / bank_transfer / card
        public decimal ReceivedAmount { get; set; }
        public decimal ChangeAmount { get; set; }
        public int? PromotionId { get; set; }
        public UserResDto User { get; set; } = null!;
        public ICollection<InvoiceDetailResDto> InvoiceDetails { get; set; } = new List<InvoiceDetailResDto>();
    }
}
