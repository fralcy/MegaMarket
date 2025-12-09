namespace MegaMarket.API.DTOs.Dashboard.Sales;

/// <summary>
/// DTO cho xu hướng doanh thu theo ngày
/// </summary>
public class RevenueTrendDto
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int InvoiceCount { get; set; }
}
