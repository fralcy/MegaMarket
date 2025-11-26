using MegaMarket.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMarket.Data.Repositories
{
    public interface IReportRepository
    {
        // Retrieves the top customers based on their total spending.
        Task<IEnumerable<Customer>> GetTopCustomersAsync(int limit);

        // Get top rewards that have been redeemed the most
        Task<IEnumerable<(int RewardId, string RewardName, string RewardType, int TotalRedeemed)>> GetTopRewardsAsync(int limit);

        // Get summary of points: total earned, redeemed, adjusted
        Task<IEnumerable<(string TransactionType, int TotalPoints)>> GetPointsSummaryAsync();
    }
}
