using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMarket.Data.Models;

public class Import
{
    [Key]
    [Column("import_id")]
    public int ImportId { get; set; }

    [ForeignKey("User")]
    [Column("user_id")]
    public int UserId { get; set; }

    [ForeignKey("Supplier")]
    [Column("supplier_id")]
    public int SupplierId { get; set; }

    [Column("import_date")]
    public DateTime ImportDate { get; set; } = DateTime.Now;

    [Column("total_cost", TypeName = "decimal(12,2)")]
    public decimal TotalCost { get; set; }

    [StringLength(20)]
    [Column("status")]
    public string Status { get; set; } = "Pending"; // Pending / Completed

    // Navigation properties
    public User User { get; set; } = null!;
    public Supplier Supplier { get; set; } = null!;
    public ICollection<ImportDetail> ImportDetails { get; set; } = new List<ImportDetail>();
}
