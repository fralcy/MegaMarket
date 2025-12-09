namespace MegaMarket.API.DTOs.Dashboard.Inventory;

/// <summary>
/// DTO cho sản phẩm bán chạy (theo số lượng)
/// </summary>
public class TopMovingProductDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int TotalSold { get; set; }
    public int CurrentStock { get; set; }
}
