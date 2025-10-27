using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMarket.Data.Models;

public class PointTransaction
{
    [Key]
    [Column("transaction_id")]
    public int TransactionId { get; set; }

    [ForeignKey("Invoice")]
    [Column("invoice_id")]
    public int? InvoiceId { get; set; }

    [ForeignKey("Customer")]
    [Column("customer_id")]
    public int CustomerId { get; set; }

    [Column("point_change")]
    public int PointChange { get; set; } // Can be positive or negative

    [StringLength(50)]
    [Column("transaction_type")]
    public string TransactionType { get; set; } = "Earn"; // Earn / Redeem / Adjust

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("description")]
    public string? Description { get; set; }

    // Navigation properties
    public Invoice? Invoice { get; set; }
    public Customer Customer { get; set; } = null!;
}
