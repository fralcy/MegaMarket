namespace MegaMarket.BlazorUI.Models.Dashboard;

public class RevenueByPaymentMethodDto
{
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int InvoiceCount { get; set; }
}
