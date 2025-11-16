namespace MegaMarket.API.DTOs.PointTransaction
{
    public class HistoryPointResponseDto
    {
        public int TransactionId { get; set; }
        public int PointChange { get; set; }
        public string? TransactionType { get; set; } // Earn / Redeem / Adjust
        public DateTime CreatedAt { get; set; }
        public string? Description { get; set; }
        public int? InvoiceId { get; set; }
    }
}
