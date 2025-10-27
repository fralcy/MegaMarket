using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMarket.Data.Models;

public class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Required]
    [StringLength(100)]
    [Column("full_name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Column("username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    [Column("password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Column("role")]
    public string Role { get; set; } = "Employee"; // Admin / Employee

    [StringLength(15)]
    [Column("phone")]
    public string? Phone { get; set; }

    [StringLength(100)]
    [Column("email")]
    public string? Email { get; set; }

    // Navigation properties
    public ICollection<Import> Imports { get; set; } = new List<Import>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}
