namespace MegaMarket.BlazorUI.Models.Dashboard;

public class EmployeeDashboardDto
{
    public int? TotalEmployees { get; set; } // Chỉ Admin mới thấy
    public int TotalAttendances { get; set; }
    public double OnTimePercentage { get; set; }
    public double LatePercentage { get; set; }
    public double TotalHoursWorked { get; set; }
    public decimal? TotalWages { get; set; } // Chỉ Admin mới thấy
    public List<AttendanceByShiftDto> AttendanceByShift { get; set; } = new();
}
