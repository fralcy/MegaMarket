using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMarket.Data.Models;

public class OrderRequest
{
    [Key]
    [Column("order_id")]
    public int OrderId { get; set; }

    [ForeignKey("Product")]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("requested_quantity")]
    public int RequestedQuantity { get; set; }

    [Column("request_date")]
    public DateTime RequestDate { get; set; } = DateTime.Now;

    [StringLength(20)]
    [Column("status")]
    public string Status { get; set; } = "Pending"; // Pending / Ordered / Received

    // Navigation properties
    public Product Product { get; set; } = null!;
}
