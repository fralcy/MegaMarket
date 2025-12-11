namespace MegaMarket.API.DTOs.Reports
{
    public class PointsSummaryDto
    {
        public string TransactionType { get; set; } = null!;  // Earn, Redeem, Adjust
        public int TotalPoints { get; set; }
    }
}
