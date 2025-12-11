namespace MegaMarket.BlazorUI.Models.Dashboard;

public class TopMovingProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}
