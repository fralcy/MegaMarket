namespace MegaMarket.API.DTOs.Dashboard.Employee;

public class EmployeeWorkSummaryDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public int DaysWorked { get; set; }
    public double TotalHoursWorked { get; set; }
    public int LateCount { get; set; }
    public decimal TotalWages { get; set; }
    public double AttendanceRate { get; set; } // % đúng giờ
}
