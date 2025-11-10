namespace MegaMarket.API.DTOs;

public class ShiftTypeInputDto
{
    public string Name { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal WagePerHour { get; set; }
}