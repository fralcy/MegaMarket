using MegaMarket.API.DTOs.Dashboard.Common;

namespace MegaMarket.API.Helpers;

public static class DateRangeHelper
{
    /// <summary>
    /// Chuyển đổi DateRangeEnum thành khoảng thời gian cụ thể
    /// </summary>
    public static DateRangePeriod GetDateRange(DateRangeEnum dateRange)
    {
        var now = DateTime.Now;
        var today = now.Date;

        return dateRange switch
        {
            DateRangeEnum.Today => new DateRangePeriod
            {
                StartDate = today,
                EndDate = today.AddDays(1).AddTicks(-1) // 23:59:59.9999999
            },

            DateRangeEnum.ThisWeek => new DateRangePeriod
            {
                StartDate = GetStartOfWeek(today),
                EndDate = GetStartOfWeek(today).AddDays(7).AddTicks(-1)
            },

            DateRangeEnum.ThisMonth => new DateRangePeriod
            {
                StartDate = new DateTime(today.Year, today.Month, 1),
                EndDate = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddTicks(-1)
            },

            DateRangeEnum.ThisYear => new DateRangePeriod
            {
                StartDate = new DateTime(today.Year, 1, 1),
                EndDate = new DateTime(today.Year, 12, 31, 23, 59, 59, 999)
            },

            _ => throw new ArgumentException($"Invalid date range: {dateRange}")
        };
    }

    /// <summary>
    /// Lấy ngày đầu tuần (Monday)
    /// </summary>
    private static DateTime GetStartOfWeek(DateTime date)
    {
        // Monday là ngày đầu tuần
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }
}
