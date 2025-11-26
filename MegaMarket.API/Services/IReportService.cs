using MegaMarket.API.DTOs.Reports;

namespace MegaMarket.API.Services
{
    public interface IReportService
    {
        // Get a list of top customers based on their points.
        Task<IEnumerable<TopCustomerDto>> GetTopCustomersAsync(int limit);

        // Get top rewards that have been redeemed the most
        Task<IEnumerable<TopRewardDto>> GetTopRewardsAsync(int limit);

        // Get summary of points: total earned, redeemed, adjusted
        Task<IEnumerable<PointsSummaryDto>> GetPointsSummaryAsync();
    }
}
