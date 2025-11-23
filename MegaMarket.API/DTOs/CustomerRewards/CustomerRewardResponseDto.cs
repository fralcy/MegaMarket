namespace MegaMarket.API.DTOs.CustomerRewards
{
    public class CustomerRewardResponseDto
    {
        public int RedemptionId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        public int RewardId { get; set; }
        public string RewardName { get; set; } = string.Empty;

        public int? InvoiceId { get; set; }
        public DateTime RedeemedAt { get; set; }

        public string Status { get; set; } = string.Empty;   // Pending / Claimed / Used
        public DateTime? UsedAt { get; set; }
    }

}
