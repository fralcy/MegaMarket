using MegaMarket.API.DTOs.Reports;
using MegaMarket.Data.Repositories;
namespace MegaMarket.API.Services
{
    public class ReportService: IReportService
    {
        private readonly IReportRepository _reportRepository;
        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        // Get a list of top customers based on their points.
        public async Task<IEnumerable<TopCustomerDto>> GetTopCustomersAsync(int limit)
        {
            var customers = await _reportRepository.GetTopCustomersAsync(limit);
            return customers.Select(c => new TopCustomerDto
            {
                CustomerId = c.CustomerId,
                FullName = $"{c.FullName}",
                Phone = c.Phone!,
                Points = c.Points,
                Rank = c.Rank
            });
        }

        // Get top rewards that have been redeemed the most
        public async Task<IEnumerable<TopRewardDto>> GetTopRewardsAsync(int limit)
        {
            var rewards = await _reportRepository.GetTopRewardsAsync(limit);
            return rewards.Select(r => new TopRewardDto
            {
                RewardId = r.RewardId,
                RewardName = r.RewardName,
                RewardType = r.RewardType,
                TotalRedeemed = r.TotalRedeemed
            });
        }

        // Get summary of points: total earned, redeemed, adjusted
        public async Task<IEnumerable<PointsSummaryDto>> GetPointsSummaryAsync()
        {
            var summary = await _reportRepository.GetPointsSummaryAsync();
            return summary.Select(s => new PointsSummaryDto
            {
                TransactionType = s.TransactionType,
                TotalPoints = s.TotalPoints >= 0 ? s.TotalPoints : -s.TotalPoints
            });
        }
    }
}
