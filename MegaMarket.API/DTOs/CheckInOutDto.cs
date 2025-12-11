namespace MegaMarket.API.DTOs;

public class CheckInOutDto
{
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
}