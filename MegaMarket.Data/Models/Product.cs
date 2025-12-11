using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMarket.Data.Models;

public class Product
{
    [Key]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Required]
    [StringLength(100)]
    [Column("barcode")]
    public string Barcode { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    [Column("category")]
    public string? Category { get; set; }

    [Column("unit_price", TypeName = "decimal(10,2)")]
    public decimal UnitPrice { get; set; }



    [Column("quantity_in_stock")]
    public int QuantityInStock { get; set; } = 0;

    [Column("min_quantity")]
    public int MinQuantity { get; set; } = 0;

    [Column("expiry_date")]
    public DateTime? ExpiryDate { get; set; }

    [Column("is_perishable")]
    public bool IsPerishable { get; set; } = false;

    // NEW
    [StringLength(500)]
    [Column("image_url")]
    public string? ImageUrl { get; set; }

    [StringLength(50)]
    [Column("unit_label")]
    public string? UnitLabel { get; set; }

    [Column("original_price", TypeName = "decimal(10,2)")]
    public decimal? OriginalPrice { get; set; }

    [Column("discount_percent")]
    public int? DiscountPercent { get; set; }



    public ICollection<ImportDetail> ImportDetails { get; set; } = new List<ImportDetail>();
    public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    public ICollection<OrderRequest> OrderRequests { get; set; } = new List<OrderRequest>();
    public ICollection<PromotionProduct> PromotionProducts { get; set; } = new List<PromotionProduct>();
}
