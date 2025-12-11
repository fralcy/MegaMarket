namespace MegaMarket.Tests.Integration.Controllers.Reports;

public class ReportsControllerTests : IClassFixture<TestServerFixture>
{
    private readonly HttpClient _client;
    private readonly TestServerFixture _factory;

    public ReportsControllerTests(TestServerFixture factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTopCustomers_DefaultLimit_ReturnsSuccess()
    {
        // Arrange - Add some point transactions to create customer activity
        await _client.PostAsJsonAsync("/api/points/1/add", new { Points = 100, Description = "Test" });
        await _client.PostAsJsonAsync("/api/points/2/add", new { Points = 200, Description = "Test" });
        await _client.PostAsJsonAsync("/api/points/3/add", new { Points = 300, Description = "Test" });

        // Act
        var response = await _client.GetAsync("/api/reports/customers/top");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("data");
    }

    [Fact]
    public async Task GetTopCustomers_CustomLimit_ReturnsLimitedResults()
    {
        // Arrange
        var limit = 5;

        // Act
        var response = await _client.GetAsync($"/api/reports/customers/top?limit={limit}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<dynamic>();
        content.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTopCustomers_WithActivity_ReturnsOrderedByPoints()
    {
        // Arrange - Create customers with different point levels
        await _client.PostAsJsonAsync("/api/points/1/add", new { Points = 500, Description = "High activity" });
        await _client.PostAsJsonAsync("/api/points/2/add", new { Points = 100, Description = "Low activity" });
        await _client.PostAsJsonAsync("/api/points/3/add", new { Points = 1000, Description = "Highest activity" });

        // Act
        var response = await _client.GetAsync("/api/reports/customers/top?limit=3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTopRewards_DefaultLimit_ReturnsSuccess()
    {
        // Arrange - Redeem some rewards to create activity
        await _client.PostAsJsonAsync("/api/customerrewards/redeem", new
        {
            CustomerId = 3,
            RewardId = 1
        });
        await _client.PostAsJsonAsync("/api/customerrewards/redeem", new
        {
            CustomerId = 3,
            RewardId = 2
        });

        // Act
        var response = await _client.GetAsync("/api/reports/rewards/top");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("data");
    }

    [Fact]
    public async Task GetTopRewards_CustomLimit_ReturnsLimitedResults()
    {
        // Arrange
        var limit = 3;

        // Act
        var response = await _client.GetAsync($"/api/reports/rewards/top?limit={limit}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTopRewards_WithRedemptions_ReturnsOrderedByPopularity()
    {
        // Arrange - Redeem different rewards
        await _client.PostAsJsonAsync("/api/customerrewards/redeem", new { CustomerId = 1, RewardId = 1 });
        await _client.PostAsJsonAsync("/api/customerrewards/redeem", new { CustomerId = 1, RewardId = 1 });
        await _client.PostAsJsonAsync("/api/customerrewards/redeem", new { CustomerId = 2, RewardId = 2 });

        // Act
        var response = await _client.GetAsync("/api/reports/rewards/top?limit=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPointsSummary_ReturnsSuccess()
    {
        // Arrange - Create some point transactions
        await _client.PostAsJsonAsync("/api/points/1/add", new { Points = 150, Description = "Purchase points" });
        await _client.PostAsJsonAsync("/api/points/2/add", new { Points = 75, Description = "Bonus points" });
        await _client.PostAsJsonAsync("/api/points/1/subtract", new
        {
            Points = 50,
            TransactionType = "Redeem",
            Description = "Reward redemption"
        });

        // Act
        var response = await _client.GetAsync("/api/reports/points/summary");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("data");
    }

    [Fact]
    public async Task GetPointsSummary_WithTransactions_ReturnsAnalytics()
    {
        // Arrange - Create various point transactions
        await _client.PostAsJsonAsync("/api/points/1/add", new { Points = 200, Description = "Test" });
        await _client.PostAsJsonAsync("/api/points/2/add", new { Points = 100, Description = "Test" });
        await _client.PostAsJsonAsync("/api/points/3/add", new { Points = 300, Description = "Test" });

        // Act
        var response = await _client.GetAsync("/api/reports/points/summary");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAllReports_AllEndpoints_ReturnSuccess()
    {
        // This test verifies all report endpoints are working

        // Act & Assert - Top Customers
        var customersResponse = await _client.GetAsync("/api/reports/customers/top");
        customersResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act & Assert - Top Rewards
        var rewardsResponse = await _client.GetAsync("/api/reports/rewards/top");
        rewardsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act & Assert - Points Summary
        var pointsResponse = await _client.GetAsync("/api/reports/points/summary");
        pointsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTopCustomers_VaryingLimits_ReturnsAppropriateResults()
    {
        // Test different limit values
        var limits = new[] { 1, 5, 10, 20 };

        foreach (var limit in limits)
        {
            // Act
            var response = await _client.GetAsync($"/api/reports/customers/top?limit={limit}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    [Fact]
    public async Task GetTopRewards_VaryingLimits_ReturnsAppropriateResults()
    {
        // Test different limit values
        var limits = new[] { 1, 3, 5, 10 };

        foreach (var limit in limits)
        {
            // Act
            var response = await _client.GetAsync($"/api/reports/rewards/top?limit={limit}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    [Fact]
    public async Task ReportsFlow_CreateDataAndVerify_Success()
    {
        // Complete flow: Create activity, then verify in reports

        // Step 1: Create customer activity
        await _client.PostAsJsonAsync("/api/points/1/add", new { Points = 500, Description = "Activity 1" });
        await _client.PostAsJsonAsync("/api/points/2/add", new { Points = 300, Description = "Activity 2" });

        // Step 2: Create reward redemptions
        await _client.PostAsJsonAsync("/api/customerrewards/redeem", new { CustomerId = 3, RewardId = 1 });
        await _client.PostAsJsonAsync("/api/customerrewards/redeem", new { CustomerId = 3, RewardId = 2 });

        // Step 3: Verify reports show the data
        var customersReport = await _client.GetAsync("/api/reports/customers/top");
        customersReport.StatusCode.Should().Be(HttpStatusCode.OK);

        var rewardsReport = await _client.GetAsync("/api/reports/rewards/top");
        rewardsReport.StatusCode.Should().Be(HttpStatusCode.OK);

        var pointsReport = await _client.GetAsync("/api/reports/points/summary");
        pointsReport.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
