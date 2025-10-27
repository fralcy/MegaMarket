using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMarket.Data.Models;

public class Invoice
{
    [Key]
    [Column("invoice_id")]
    public int InvoiceId { get; set; }

    [ForeignKey("User")]
    [Column("user_id")]
    public int UserId { get; set; }

    [ForeignKey("Customer")]
    [Column("customer_id")]
    public int? CustomerId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("total_before_discount", TypeName = "decimal(12,2)")]
    public decimal TotalBeforeDiscount { get; set; }

    [Column("total_amount", TypeName = "decimal(12,2)")]
    public decimal TotalAmount { get; set; }

    [StringLength(50)]
    [Column("payment_method")]
    public string PaymentMethod { get; set; } = "cash"; // cash / bank_transfer / card

    [Column("received_amount", TypeName = "decimal(12,2)")]
    public decimal ReceivedAmount { get; set; }

    [Column("change_amount", TypeName = "decimal(12,2)")]
    public decimal ChangeAmount { get; set; }

    [StringLength(20)]
    [Column("status")]
    public string Status { get; set; } = "Pending"; // Paid / Pending

    [ForeignKey("Promotion")]
    [Column("promotion_id")]
    public int? PromotionId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Customer? Customer { get; set; }
    public Promotion? Promotion { get; set; }
    public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    public ICollection<PointTransaction> PointTransactions { get; set; } = new List<PointTransaction>();
}
