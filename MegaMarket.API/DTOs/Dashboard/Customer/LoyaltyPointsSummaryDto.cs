namespace MegaMarket.API.DTOs.Dashboard.Customer;

/// <summary>
/// DTO cho tổng kết điểm thưởng và loyalty program
/// </summary>
public class LoyaltyPointsSummaryDto
{
    public int TotalPointsInSystem { get; set; }
    public int PointsEarnedInPeriod { get; set; }
    public int PointsRedeemedInPeriod { get; set; }
    public int ActiveRewardsCount { get; set; }
    public int RewardsRedeemedInPeriod { get; set; }
}
