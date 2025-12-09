namespace MegaMarket.BlazorUI.Models.Dashboard;

public class CategoryStockDto
{
    public string CategoryName { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public int ProductCount { get; set; }
}
