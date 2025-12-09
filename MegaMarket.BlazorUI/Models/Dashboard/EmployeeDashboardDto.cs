namespace MegaMarket.BlazorUI.Models.Dashboard;

public class EmployeeDashboardDto
{
    public int TotalEmployees { get; set; }
    public int ActiveShifts { get; set; }
    public decimal? TotalWages { get; set; } // Nullable for Employee role
    public List<AttendanceByShiftDto> AttendanceByShift { get; set; } = new();
    public List<EmployeeWorkSummaryDto> EmployeeWorkSummary { get; set; } = new();
}
