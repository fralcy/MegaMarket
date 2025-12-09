using Microsoft.EntityFrameworkCore;
using MegaMarket.Data.Data;
using MegaMarket.API.DTOs.Dashboard.Inventory;

namespace MegaMarket.API.Services;

public class DashboardInventoryService
{
    private readonly MegaMarketDbContext _context;

    public DashboardInventoryService(MegaMarketDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy tổng quan dashboard tồn kho
    /// </summary>
    public async Task<InventoryDashboardDto> GetInventoryDashboardAsync(string? role)
    {
        var totalProducts = await _context.Products.CountAsync();

        var totalCategories = await _context.Products
            .Where(p => p.Category != null)
            .Select(p => p.Category)
            .Distinct()
            .CountAsync();

        var lowStockCount = await _context.Products
            .CountAsync(p => p.QuantityInStock <= p.MinQuantity);

        var outOfStockCount = await _context.Products
            .CountAsync(p => p.QuantityInStock == 0);

        // Chỉ Admin mới xem được giá trị tồn kho
        decimal? totalStockValue = null;
        if (role == "Admin")
        {
            totalStockValue = await _context.Products
                .SumAsync(p => p.QuantityInStock * p.UnitPrice);
        }

        // Sản phẩm sắp hết hạn (trong 30 ngày)
        var threshold = DateTime.Now.AddDays(30);
        var productsExpiringSoon = await _context.Products
            .CountAsync(p => p.IsPerishable &&
                            p.ExpiryDate != null &&
                            p.ExpiryDate <= threshold &&
                            p.ExpiryDate >= DateTime.Now);

        return new InventoryDashboardDto
        {
            TotalProducts = totalProducts,
            TotalCategories = totalCategories,
            LowStockCount = lowStockCount,
            OutOfStockCount = outOfStockCount,
            TotalStockValue = totalStockValue,
            ProductsExpiringSoon = productsExpiringSoon
        };
    }

    /// <summary>
    /// Lấy danh sách sản phẩm sắp hết hàng
    /// </summary>
    public async Task<List<LowStockProductDto>> GetLowStockProductsAsync(int limit)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.QuantityInStock <= p.MinQuantity)
            .OrderBy(p => p.QuantityInStock)
            .Take(limit)
            .Select(p => new LowStockProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Category = p.Category,
                QuantityInStock = p.QuantityInStock,
                MinQuantity = p.MinQuantity,
                ShortageAmount = p.MinQuantity - p.QuantityInStock
            })
            .ToListAsync();
    }

    /// <summary>
    /// Lấy danh sách sản phẩm sắp hết hạn
    /// </summary>
    public async Task<List<ExpiringProductDto>> GetExpiringProductsAsync(int daysThreshold, int limit)
    {
        var threshold = DateTime.Now.AddDays(daysThreshold);
        var now = DateTime.Now;

        return await _context.Products
            .AsNoTracking()
            .Where(p => p.IsPerishable &&
                       p.ExpiryDate != null &&
                       p.ExpiryDate <= threshold &&
                       p.ExpiryDate >= now)
            .OrderBy(p => p.ExpiryDate)
            .Take(limit)
            .Select(p => new ExpiringProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Category = p.Category,
                ExpiryDate = p.ExpiryDate,
                DaysUntilExpiry = p.ExpiryDate.HasValue
                    ? (int)(p.ExpiryDate.Value - now).TotalDays
                    : 0,
                QuantityInStock = p.QuantityInStock
            })
            .ToListAsync();
    }

    /// <summary>
    /// Lấy thống kê tồn kho theo danh mục
    /// </summary>
    public async Task<List<CategoryStockDto>> GetStockByCategoryAsync(string? role)
    {
        var query = _context.Products
            .AsNoTracking()
            .Where(p => p.Category != null)
            .GroupBy(p => p.Category)
            .Select(g => new CategoryStockDto
            {
                Category = g.Key!,
                ProductCount = g.Count(),
                TotalQuantity = g.Sum(p => p.QuantityInStock),
                TotalValue = role == "Admin"
                    ? g.Sum(p => p.QuantityInStock * p.UnitPrice)
                    : (decimal?)null
            })
            .OrderByDescending(x => x.TotalQuantity);

        return await query.ToListAsync();
    }

    /// <summary>
    /// Lấy top sản phẩm bán chạy (30 ngày gần nhất)
    /// </summary>
    public async Task<List<TopMovingProductDto>> GetTopMovingProductsAsync(int limit)
    {
        var thirtyDaysAgo = DateTime.Now.AddDays(-30);

        return await _context.InvoiceDetails
            .AsNoTracking()
            .Include(id => id.Invoice)
            .Include(id => id.Product)
            .Where(id => id.Invoice.Status == "Paid" &&
                        id.Invoice.CreatedAt >= thirtyDaysAgo)
            .GroupBy(id => new { id.ProductId, id.Product.Name, id.Product.Category, id.Product.QuantityInStock })
            .Select(g => new TopMovingProductDto
            {
                ProductId = g.Key.ProductId,
                Name = g.Key.Name,
                Category = g.Key.Category,
                TotalSold = g.Sum(id => id.Quantity),
                CurrentStock = g.Key.QuantityInStock
            })
            .OrderByDescending(x => x.TotalSold)
            .Take(limit)
            .ToListAsync();
    }
}
