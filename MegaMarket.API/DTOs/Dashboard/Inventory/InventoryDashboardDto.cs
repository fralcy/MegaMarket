namespace MegaMarket.API.DTOs.Dashboard.Inventory;

/// <summary>
/// DTO cho tổng quan dashboard tồn kho
/// </summary>
public class InventoryDashboardDto
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int LowStockCount { get; set; }
    public int OutOfStockCount { get; set; }
    public decimal? TotalStockValue { get; set; } // Null cho Employee
    public int ProductsExpiringSoon { get; set; }
}
