using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMarket.Data.Models;

public class PromotionProduct
{
    [Key, Column(Order = 0)]
    [ForeignKey("Promotion")]
    [Column("promotion_id")]
    public int PromotionId { get; set; }

    [Key, Column(Order = 1)]
    [ForeignKey("Product")]
    [Column("product_id")]
    public int ProductId { get; set; }

    // Navigation properties
    public Promotion Promotion { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
