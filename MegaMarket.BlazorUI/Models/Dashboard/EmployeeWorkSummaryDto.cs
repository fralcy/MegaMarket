namespace MegaMarket.BlazorUI.Models.Dashboard;

public class EmployeeWorkSummaryDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int WorkingDays { get; set; }
    public int AbsentDays { get; set; }
    public decimal TotalHours { get; set; }
}
