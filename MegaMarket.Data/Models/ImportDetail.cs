using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMarket.Data.Models;

public class ImportDetail
{
    [Key, Column(Order = 0)]
    [ForeignKey("Import")]
    [Column("import_id")]
    public int ImportId { get; set; }

    [Key, Column(Order = 1)]
    [ForeignKey("Product")]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("unit_price", TypeName = "decimal(10,2)")]
    public decimal UnitPrice { get; set; }

    [Column("expiry_date")]
    public DateTime? ExpiryDate { get; set; }

    // Navigation properties
    public Import Import { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
