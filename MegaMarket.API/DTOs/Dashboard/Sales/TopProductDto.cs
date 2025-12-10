namespace MegaMarket.API.DTOs.Dashboard.Sales;

/// <summary>
/// DTO cho sản phẩm bán chạy nhất
/// </summary>
public class TopProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}
