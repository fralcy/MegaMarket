namespace MegaMarket.API.DTOs.Dashboard.Sales;

/// <summary>
/// DTO cho doanh thu theo phương thức thanh toán
/// </summary>
public class RevenueByPaymentMethodDto
{
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public int InvoiceCount { get; set; }
    public decimal Percentage { get; set; }
}
