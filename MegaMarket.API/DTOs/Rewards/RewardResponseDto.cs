namespace MegaMarket.API.DTOs.Rewards
{
    public class RewardResponseDto
    {
        public int RewardId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int PointCost { get; set; }
        public string? RewardType { get; set; }
        public decimal? Value { get; set; }
        public int QuantityAvailable { get; set; }
        public bool IsActive { get; set; }

    }
}
