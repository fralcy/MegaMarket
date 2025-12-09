using Microsoft.EntityFrameworkCore;
using MegaMarket.Data.Data;
using MegaMarket.API.DTOs.Dashboard.Common;
using MegaMarket.API.DTOs.Dashboard.Customer;
using MegaMarket.API.Helpers;

namespace MegaMarket.API.Services;

public class DashboardCustomerService
{
    private readonly MegaMarketDbContext _context;

    public DashboardCustomerService(MegaMarketDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy tổng quan dashboard khách hàng và loyalty
    /// </summary>
    public async Task<CustomerDashboardDto> GetCustomerDashboardAsync(DateRangeEnum dateRange, string? role)
    {
        var period = DateRangeHelper.GetDateRange(dateRange);

        var totalCustomers = await _context.Customers.CountAsync();

        // Khách hàng mới: những người có hóa đơn đầu tiên trong khoảng thời gian
        var firstPurchases = await _context.Invoices
            .Where(i => i.CustomerId != null)
            .GroupBy(i => i.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                FirstPurchase = g.Min(i => i.CreatedAt)
            })
            .Where(x => x.FirstPurchase >= period.StartDate &&
                       x.FirstPurchase <= period.EndDate)
            .CountAsync();

        // Phân bố xếp hạng
        var rankDistribution = await GetCustomerRankDistributionAsync();

        // Loyalty summary chỉ cho Admin
        LoyaltyPointsSummaryDto? loyaltySummary = null;
        if (role == "Admin")
        {
            loyaltySummary = await GetLoyaltyPointsSummaryAsync(period);
        }

        return new CustomerDashboardDto
        {
            TotalCustomers = totalCustomers,
            NewCustomers = firstPurchases,
            RankDistribution = rankDistribution,
            LoyaltyPointsSummary = loyaltySummary
        };
    }

    /// <summary>
    /// Lấy phân bố xếp hạng khách hàng
    /// </summary>
    public async Task<List<CustomerRankDistributionDto>> GetCustomerRankDistributionAsync()
    {
        var totalCustomers = await _context.Customers.CountAsync();

        var distribution = await _context.Customers
            .AsNoTracking()
            .GroupBy(c => c.Rank)
            .Select(g => new CustomerRankDistributionDto
            {
                Rank = g.Key,
                Count = g.Count(),
                Percentage = totalCustomers > 0 ? (decimal)g.Count() / totalCustomers * 100 : 0
            })
            .OrderBy(x => x.Rank)
            .ToListAsync();

        return distribution;
    }

    /// <summary>
    /// Lấy top khách hàng theo doanh thu (Admin only)
    /// </summary>
    public async Task<List<TopCustomerDto>> GetTopCustomersByRevenueAsync(DateRangeEnum dateRange, int limit)
    {
        var period = DateRangeHelper.GetDateRange(dateRange);

        var topCustomers = await _context.Invoices
            .AsNoTracking()
            .Include(i => i.Customer)
            .Where(i => i.CustomerId != null &&
                       i.Status == "Paid" &&
                       i.CreatedAt >= period.StartDate &&
                       i.CreatedAt <= period.EndDate)
            .GroupBy(i => new
            {
                i.CustomerId,
                i.Customer!.FullName,
                i.Customer.Phone,
                i.Customer.Rank
            })
            .Select(g => new TopCustomerDto
            {
                CustomerId = g.Key.CustomerId!.Value,
                FullName = g.Key.FullName,
                Phone = g.Key.Phone,
                Rank = g.Key.Rank,
                TotalSpent = g.Sum(i => i.TotalAmount),
                InvoiceCount = g.Count(),
                AvgOrderValue = g.Average(i => i.TotalAmount)
            })
            .OrderByDescending(x => x.TotalSpent)
            .Take(limit)
            .ToListAsync();

        return topCustomers;
    }

    /// <summary>
    /// Lấy tổng kết loyalty program (private helper)
    /// </summary>
    private async Task<LoyaltyPointsSummaryDto> GetLoyaltyPointsSummaryAsync(DateRangePeriod period)
    {
        var totalPointsInSystem = await _context.Customers.SumAsync(c => c.Points);

        var pointsEarned = await _context.PointTransactions
            .Where(pt => pt.TransactionType == "Earn" &&
                        pt.CreatedAt >= period.StartDate &&
                        pt.CreatedAt <= period.EndDate)
            .SumAsync(pt => pt.PointChange);

        var pointsRedeemed = await _context.PointTransactions
            .Where(pt => pt.TransactionType == "Redeem" &&
                        pt.CreatedAt >= period.StartDate &&
                        pt.CreatedAt <= period.EndDate)
            .SumAsync(pt => Math.Abs(pt.PointChange));

        var activeRewardsCount = await _context.Rewards
            .CountAsync(r => r.IsActive && r.QuantityAvailable > 0);

        var rewardsRedeemed = await _context.CustomerRewards
            .CountAsync(cr => cr.RedeemedAt >= period.StartDate &&
                             cr.RedeemedAt <= period.EndDate);

        return new LoyaltyPointsSummaryDto
        {
            TotalPointsInSystem = totalPointsInSystem,
            PointsEarnedInPeriod = pointsEarned,
            PointsRedeemedInPeriod = pointsRedeemed,
            ActiveRewardsCount = activeRewardsCount,
            RewardsRedeemedInPeriod = rewardsRedeemed
        };
    }
}
