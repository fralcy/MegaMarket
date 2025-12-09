namespace MegaMarket.API.DTOs.Dashboard.Customer;

/// <summary>
/// DTO cho top khách hàng theo doanh thu
/// </summary>
public class TopCustomerDto
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Rank { get; set; } = string.Empty;
    public decimal TotalSpent { get; set; }
    public int InvoiceCount { get; set; }
    public decimal AvgOrderValue { get; set; }
}
