namespace MegaMarket.BlazorUI.Models.Dashboard;

public class AttendanceByShiftDto
{
    public string ShiftName { get; set; } = string.Empty;
    public int AttendanceCount { get; set; }
    public int OnTimeCount { get; set; }
    public int LateCount { get; set; }
    public double TotalHoursWorked { get; set; }
    public decimal TotalWages { get; set; }
}
