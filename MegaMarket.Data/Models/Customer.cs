using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMarket.Data.Models;

public class Customer
{
    [Key]
    [Column("customer_id")]
    public int CustomerId { get; set; }

    [Required]
    [StringLength(100)]
    [Column("full_name")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(15)]
    [Column("phone")]
    public string? Phone { get; set; }

    [StringLength(100)]
    [Column("email")]
    public string? Email { get; set; }

    [Column("points")]
    public int Points { get; set; } = 0;

    [StringLength(20)]
    [Column("rank")]
    public string Rank { get; set; } = "Silver"; // Silver / Gold / Platinum

    // Navigation properties
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<CustomerReward> CustomerRewards { get; set; } = new List<CustomerReward>();
    public ICollection<PointTransaction> PointTransactions { get; set; } = new List<PointTransaction>();
}
