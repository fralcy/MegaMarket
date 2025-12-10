namespace MegaMarket.API.DTOs.Dashboard.Inventory;

/// <summary>
/// DTO cho thống kê tồn kho theo danh mục
/// </summary>
public class CategoryStockDto
{
    public string Category { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public int TotalQuantity { get; set; }
    public decimal? TotalValue { get; set; } // Null cho Employee
}
