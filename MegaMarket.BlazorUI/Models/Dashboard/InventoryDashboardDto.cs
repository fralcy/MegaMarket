namespace MegaMarket.BlazorUI.Models.Dashboard;

public class InventoryDashboardDto
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public decimal? TotalStockValue { get; set; } // Nullable for Employee role
    public int LowStockCount { get; set; }
    public int OutOfStockCount { get; set; }
    public List<LowStockProductDto> LowStockProducts { get; set; } = new();
    public List<ExpiringProductDto> ExpiringProducts { get; set; } = new();
    public List<CategoryStockDto> StockByCategory { get; set; } = new();
}
