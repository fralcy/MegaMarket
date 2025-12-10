namespace MegaMarket.BlazorUI.Models.Dashboard;

public class LowStockProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}
