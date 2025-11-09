using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMarket.Data.Models;

public class PromotionProduct
{
    [Key, Column("promotion_id", Order = 0)]
    [ForeignKey("Promotion")]
    public int PromotionId { get; set; }

    [Key, Column("product_id", Order = 1)]
    [ForeignKey("Product")]
    public int ProductId { get; set; }

    // Navigation properties
    public Promotion Promotion { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
