namespace MegaMarket.API.DTOs;

public class AttendanceInputDto
{
    public int UserId { get; set; }
    public int ShiftTypeId { get; set; }
    public DateTime Date { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public string? Note { get; set; }
}