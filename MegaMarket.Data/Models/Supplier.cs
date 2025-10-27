using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMarket.Data.Models;

public class Supplier
{
    [Key]
    [Column("supplier_id")]
    public int SupplierId { get; set; }

    [Required]
    [StringLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(255)]
    [Column("address")]
    public string? Address { get; set; }

    [StringLength(100)]
    [Column("email")]
    public string? Email { get; set; }

    [StringLength(15)]
    [Column("phone")]
    public string? Phone { get; set; }

    // Navigation properties
    public ICollection<Import> Imports { get; set; } = new List<Import>();
}
