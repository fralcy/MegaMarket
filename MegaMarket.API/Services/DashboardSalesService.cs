using Microsoft.EntityFrameworkCore;
using MegaMarket.Data.Data;
using MegaMarket.API.DTOs.Dashboard.Common;
using MegaMarket.API.DTOs.Dashboard.Sales;
using MegaMarket.API.Helpers;

namespace MegaMarket.API.Services;

public class DashboardSalesService
{
    private readonly MegaMarketDbContext _context;

    public DashboardSalesService(MegaMarketDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy tổng quan dashboard doanh thu
    /// </summary>
    public async Task<SalesDashboardDto> GetSalesDashboardAsync(DateRangeEnum dateRange, string? role, int userId)
    {
        var period = DateRangeHelper.GetDateRange(dateRange);

        // Query invoices trong khoảng thời gian
        var query = _context.Invoices
            .AsNoTracking()
            .Where(i => i.Status == "Paid" &&
                        i.CreatedAt >= period.StartDate &&
                        i.CreatedAt <= period.EndDate);

        // Employee chỉ xem được doanh thu của chính mình
        if (role != "Admin")
        {
            query = query.Where(i => i.UserId == userId);
        }

        var invoices = await query.ToListAsync();

        // Tính toán metrics
        var totalRevenue = invoices.Sum(i => i.TotalAmount);
        var invoiceCount = invoices.Count;
        var averageOrderValue = invoiceCount > 0 ? totalRevenue / invoiceCount : 0;
        var totalBeforeDiscount = invoices.Sum(i => i.TotalBeforeDiscount);
        var totalDiscountGiven = totalBeforeDiscount - totalRevenue;

        // Doanh thu theo phương thức thanh toán
        var revenueByPaymentMethod = invoices
            .GroupBy(i => i.PaymentMethod)
            .Select(g => new RevenueByPaymentMethodDto
            {
                PaymentMethod = g.Key,
                TotalRevenue = g.Sum(i => i.TotalAmount),
                InvoiceCount = g.Count(),
                Percentage = totalRevenue > 0 ? (g.Sum(i => i.TotalAmount) / totalRevenue * 100) : 0
            })
            .OrderByDescending(x => x.TotalRevenue)
            .ToList();

        // Promotion impact
        var invoicesWithPromotions = invoices.Count(i => i.PromotionId != null);
        var promotionDiscountTotal = invoices.Where(i => i.PromotionId != null)
            .Sum(i => i.TotalBeforeDiscount - i.TotalAmount);

        return new SalesDashboardDto
        {
            TotalRevenue = totalRevenue,
            InvoiceCount = invoiceCount,
            AverageOrderValue = averageOrderValue,
            TotalBeforeDiscount = totalBeforeDiscount,
            TotalDiscountGiven = totalDiscountGiven,
            RevenueByPaymentMethod = revenueByPaymentMethod,
            InvoicesWithPromotions = invoicesWithPromotions,
            PromotionDiscountTotal = promotionDiscountTotal
        };
    }

    /// <summary>
    /// Lấy xu hướng doanh thu theo ngày (Admin only)
    /// </summary>
    public async Task<List<RevenueTrendDto>> GetRevenueTrendAsync(DateRangeEnum dateRange)
    {
        var period = DateRangeHelper.GetDateRange(dateRange);

        var dailyRevenue = await _context.Invoices
            .AsNoTracking()
            .Where(i => i.Status == "Paid" &&
                        i.CreatedAt >= period.StartDate &&
                        i.CreatedAt <= period.EndDate)
            .GroupBy(i => i.CreatedAt.Date)
            .Select(g => new RevenueTrendDto
            {
                Date = g.Key,
                Revenue = g.Sum(i => i.TotalAmount),
                InvoiceCount = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        return dailyRevenue;
    }

    /// <summary>
    /// Lấy top sản phẩm bán chạy nhất
    /// </summary>
    public async Task<List<TopProductDto>> GetTopSellingProductsAsync(DateRangeEnum dateRange, int limit, string? role)
    {
        var period = DateRangeHelper.GetDateRange(dateRange);

        var topProducts = await _context.InvoiceDetails
            .AsNoTracking()
            .Include(id => id.Invoice)
            .Include(id => id.Product)
            .Where(id => id.Invoice.Status == "Paid" &&
                         id.Invoice.CreatedAt >= period.StartDate &&
                         id.Invoice.CreatedAt <= period.EndDate)
            .GroupBy(id => new { id.ProductId, id.Product.Name, id.Product.Category })
            .Select(g => new TopProductDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                Category = g.Key.Category,
                TotalQuantitySold = g.Sum(id => id.Quantity),
                TotalRevenue = g.Sum(id => id.Quantity * id.UnitPrice - id.DiscountPerUnit * id.Quantity)
            })
            .OrderByDescending(x => x.TotalRevenue)
            .Take(limit)
            .ToListAsync();

        return topProducts;
    }
}
