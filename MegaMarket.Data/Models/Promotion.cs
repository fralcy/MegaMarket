using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMarket.Data.Models;

public class Promotion
{
    [Key]
    [Column("promotion_id")]
    public int PromotionId { get; set; }

    [Required]
    [StringLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [StringLength(20)]
    [Column("discount_type")]
    public string DiscountType { get; set; } = "percent"; // percent / fixed

    [Column("discount_value", TypeName = "decimal(10,2)")]
    public decimal DiscountValue { get; set; }

    [Column("start_date")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Column("end_date")]
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(7);

    [StringLength(50)]
    [Column("type")]
    public string Type { get; set; } = "invoice"; // invoice / product / promotion

    // Navigation properties
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    public ICollection<PromotionProduct> PromotionProducts { get; set; } = new List<PromotionProduct>();
}
