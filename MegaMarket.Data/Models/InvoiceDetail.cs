using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMarket.Data.Models;

public class InvoiceDetail
{
    [Key, Column(Order = 0)]
    [ForeignKey("Invoice")]
    [Column("invoice_id")]
    public int InvoiceId { get; set; }

    [Key, Column(Order = 1)]
    [ForeignKey("Product")]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("unit_price", TypeName = "decimal(10,2)")]
    public decimal UnitPrice { get; set; }

    [Column("discount_per_unit", TypeName = "decimal(10,2)")]
    public decimal DiscountPerUnit { get; set; } = 0;

    [ForeignKey("Promotion")]
    [Column("promotion_id")]
    public int? PromotionId { get; set; }

    // Navigation properties
    public Invoice Invoice { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public Promotion? Promotion { get; set; }
}
