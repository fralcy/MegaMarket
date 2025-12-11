namespace MegaMarket.API.DTOs.Reports
{
    public class TopRewardDto
    {
        public int RewardId { get; set; }
        public string RewardName { get; set; } = null!;
        public string RewardType { get; set; } = null!;
        public int TotalRedeemed { get; set; }
    }
}
