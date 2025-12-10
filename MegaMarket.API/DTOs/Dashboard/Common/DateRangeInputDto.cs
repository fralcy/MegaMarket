namespace MegaMarket.API.DTOs.Dashboard.Common;

/// <summary>
/// Enum cho các khoảng thời gian cố định
/// </summary>
public enum DateRangeEnum
{
    Today,
    ThisWeek,
    ThisMonth,
    ThisYear
}

/// <summary>
/// Khoảng thời gian với ngày bắt đầu và kết thúc cụ thể
/// </summary>
public class DateRangePeriod
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
