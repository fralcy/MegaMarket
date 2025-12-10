namespace MegaMarket.BlazorUI.Models.Dashboard;

public class AttendanceRateDto
{
    public int TotalWorkingDays { get; set; }
    public int AttendedDays { get; set; }
    public int AbsentDays { get; set; }
    public int LateDays { get; set; }
    public decimal AttendanceRate { get; set; }
}
