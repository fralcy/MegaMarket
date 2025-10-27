using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMarket.Data.Models;

public class Reward
{
    [Key]
    [Column("reward_id")]
    public int RewardId { get; set; }

    [Required]
    [StringLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("point_cost")]
    public int PointCost { get; set; }

    [StringLength(20)]
    [Column("reward_type")]
    public string RewardType { get; set; } = "Gift"; // Gift / Voucher / Discount

    [Column("value", TypeName = "decimal(10,2)")]
    public decimal? Value { get; set; }

    [Column("quantity_available")]
    public int QuantityAvailable { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<CustomerReward> CustomerRewards { get; set; } = new List<CustomerReward>();
}
