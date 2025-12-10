using MegaMarket.Data.Data;
using MegaMarket.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMarket.Data.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly MegaMarketDbContext _context;
        public ReportRepository(MegaMarketDbContext context)
        {
            _context = context;
        }

        // Get top N customers by points
        public async Task<IEnumerable<Customer>> GetTopCustomersAsync(int limit)
        {
            return await _context.Customers
                .OrderByDescending(c => c.Points)
                .Take(limit)
                .ToListAsync();
        }

        // Get top rewards that have been redeemed the most
        public async Task<IEnumerable<(int RewardId, string RewardName, string RewardType, int TotalRedeemed)>> GetTopRewardsAsync(int limit)
        {
            var result = await _context.CustomerRewards
                .Include(cr => cr.Reward)
                .GroupBy(cr => cr.RewardId)
                .Select(g => new
                {
                    RewardId = g.Key,
                    RewardName = g.First().Reward.Name,
                    RewardType = g.First().Reward.RewardType,
                    TotalRedeemed = g.Count()
                })
                .OrderByDescending(x => x.TotalRedeemed)
                .Take(limit)
                .ToListAsync();

            return result.Select(x => (x.RewardId, x.RewardName, x.RewardType, x.TotalRedeemed));
        }

        // Get summary of points: total earned, redeemed, adjusted by transaction type
        public async Task<IEnumerable<(string TransactionType, int TotalPoints)>> GetPointsSummaryAsync()
        {
            var result = await _context.PointTransactions
                .GroupBy(pt => pt.TransactionType)
                .Select(g => new
                {
                    TransactionType = g.Key,
                    TotalPoints = g.Sum(pt => pt.PointChange)
                })
                .ToListAsync();

            return result.Select(x => (x.TransactionType, x.TotalPoints));
        }
    }
}
