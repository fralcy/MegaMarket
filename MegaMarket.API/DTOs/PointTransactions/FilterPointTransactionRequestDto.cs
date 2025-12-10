namespace MegaMarket.API.DTOs.PointTransaction
{
    public class FilterPointTransactionRequestDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? TransactionType { get; set; }  // Earn / Redeem / Adjust
    }
}
