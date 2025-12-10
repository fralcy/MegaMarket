namespace MegaMarket.BlazorUI.Models.Dashboard;

public class SalesDashboardDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalInvoices { get; set; }
    public decimal AverageOrderValue { get; set; }
    public List<RevenueByPaymentMethodDto> RevenueByPaymentMethod { get; set; } = new();
    public List<RevenueTrendDto> RevenueTrend { get; set; } = new();
    public List<TopProductDto> TopSellingProducts { get; set; } = new();
}
