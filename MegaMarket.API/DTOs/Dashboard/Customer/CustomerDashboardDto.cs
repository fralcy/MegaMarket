namespace MegaMarket.API.DTOs.Dashboard.Customer;

/// <summary>
/// DTO cho tổng quan dashboard khách hàng
/// </summary>
public class CustomerDashboardDto
{
    public int TotalCustomers { get; set; }
    public int NewCustomers { get; set; } // Khách hàng mới trong khoảng thời gian
    public List<CustomerRankDistributionDto> RankDistribution { get; set; } = new();
    public LoyaltyPointsSummaryDto? LoyaltyPointsSummary { get; set; } // Null cho Employee
}
