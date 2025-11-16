using System.ComponentModel.DataAnnotations;

namespace MegaMarket.API.DTOs.Rewards
{
    public class CreateRewardRequestDto
    {
        public string? Name { get; set; } = null!;
        public string? Description { get; set; }

        [Range(1,int.MaxValue)]
        public int PointCost { get; set; }
        public string? RewardType { get; set; } = null!;
        public decimal? Value { get; set; }
        [Range (0,int.MaxValue)]
        public int QuantityAvailable { get; set; }
    }
}
