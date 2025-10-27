using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMarket.Data.Models;

public class CustomerReward
{
    [Key]
    [Column("redemption_id")]
    public int RedemptionId { get; set; }

    [ForeignKey("Customer")]
    [Column("customer_id")]
    public int CustomerId { get; set; }

    [ForeignKey("Reward")]
    [Column("reward_id")]
    public int RewardId { get; set; }

    [ForeignKey("Invoice")]
    [Column("invoice_id")]
    public int? InvoiceId { get; set; }

    [Column("redeemed_at")]
    public DateTime RedeemedAt { get; set; } = DateTime.Now;

    [StringLength(20)]
    [Column("status")]
    public string Status { get; set; } = "Pending"; // Pending / Claimed / Used

    [Column("used_at")]
    public DateTime? UsedAt { get; set; }

    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public Reward Reward { get; set; } = null!;
    public Invoice? Invoice { get; set; }
}
