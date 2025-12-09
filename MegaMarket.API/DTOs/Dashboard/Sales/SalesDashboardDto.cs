namespace MegaMarket.API.DTOs.Dashboard.Sales;

/// <summary>
/// DTO cho tổng quan dashboard doanh thu
/// </summary>
public class SalesDashboardDto
{
    public decimal TotalRevenue { get; set; }
    public int InvoiceCount { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal TotalBeforeDiscount { get; set; }
    public decimal TotalDiscountGiven { get; set; }
    public List<RevenueByPaymentMethodDto> RevenueByPaymentMethod { get; set; } = new();
    public int InvoicesWithPromotions { get; set; }
    public decimal PromotionDiscountTotal { get; set; }
}
