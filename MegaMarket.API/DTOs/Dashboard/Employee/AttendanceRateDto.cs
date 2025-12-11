namespace MegaMarket.API.DTOs.Dashboard.Employee;

public class AttendanceRateDto
{
    public int TotalAttendances { get; set; }
    public int OnTimeCount { get; set; }
    public int LateCount { get; set; }
    public double OnTimePercentage { get; set; }
    public double LatePercentage { get; set; }
    public double TotalHoursWorked { get; set; }
    public double TotalWages { get; set; }
}
