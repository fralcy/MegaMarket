namespace MegaMarket.BlazorUI.Models.Dashboard;

public class CustomerDashboardDto
{
    public int TotalCustomers { get; set; }
    public int NewCustomers { get; set; }
    public decimal AverageSpending { get; set; }
    public List<CustomerRankDistributionDto> CustomerRankDistribution { get; set; } = new();
    public LoyaltyPointsSummaryDto? LoyaltyPointsSummary { get; set; } // Nullable for Employee role
    public List<TopCustomerDto> TopCustomers { get; set; } = new();
}
