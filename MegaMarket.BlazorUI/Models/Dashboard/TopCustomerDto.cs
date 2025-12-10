namespace MegaMarket.BlazorUI.Models.Dashboard;

public class TopCustomerDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public decimal TotalSpending { get; set; }
    public int InvoiceCount { get; set; }
}
