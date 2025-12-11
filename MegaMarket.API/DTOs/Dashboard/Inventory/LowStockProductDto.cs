namespace MegaMarket.API.DTOs.Dashboard.Inventory;

/// <summary>
/// DTO cho sản phẩm sắp hết hàng
/// </summary>
public class LowStockProductDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int QuantityInStock { get; set; }
    public int MinQuantity { get; set; }
    public int ShortageAmount { get; set; }
}
