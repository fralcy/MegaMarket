using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaMarket.Data.Models;

public class Attendance
{
    [Key]
    [Column("attendance_id")]
    public int AttendanceId { get; set; }

    [ForeignKey("User")]
    [Column("user_id")]
    public int UserId { get; set; }

    [ForeignKey("ShiftType")]
    [Column("shift_type_id")]
    public int ShiftTypeId { get; set; }

    [Column("date")]
    public DateTime Date { get; set; }

    [Column("check_in")]
    public DateTime? CheckIn { get; set; }

    [Column("check_out")]
    public DateTime? CheckOut { get; set; }

    [Column("is_late")]
    public bool IsLate { get; set; } = false;

    [Column("note")]
    public string? Note { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ShiftType ShiftType { get; set; } = null!;
}
