using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMarket.Data.Models;

public class ShiftType
{
    [Key]
    [Column("shift_type_id")]
    public int ShiftTypeId { get; set; }

    [Required]
    [StringLength(50)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("start_time")]
    public TimeSpan StartTime { get; set; }

    [Column("end_time")]
    public TimeSpan EndTime { get; set; }

    [Column("wage_per_hour", TypeName = "decimal(10,2)")]
    public decimal WagePerHour { get; set; }

    // Navigation properties
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}
