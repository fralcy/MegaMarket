namespace MegaMarket.API.DTOs.Dashboard.Inventory;

/// <summary>
/// DTO cho sản phẩm sắp hết hạn
/// </summary>
public class ExpiringProductDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int DaysUntilExpiry { get; set; }
    public int QuantityInStock { get; set; }
}
