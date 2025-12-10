namespace MegaMarket.API.DTOs.CustomerRewards
{
    public class RedeemRewardRequestDto
    {
        public int CustomerId { get; set; }
        public int RewardId { get; set; }
        public int? InvoiceId { get; set; }  // Optional: liên k?t v?i Invoice n?u có
    }
}
