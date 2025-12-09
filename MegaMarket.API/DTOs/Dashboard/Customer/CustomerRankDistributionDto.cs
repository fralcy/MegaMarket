namespace MegaMarket.API.DTOs.Dashboard.Customer;

/// <summary>
/// DTO cho phân bố xếp hạng khách hàng
/// </summary>
public class CustomerRankDistributionDto
{
    public string Rank { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}
