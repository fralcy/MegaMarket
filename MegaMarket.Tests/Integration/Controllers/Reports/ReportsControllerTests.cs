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
    public async Task GetTopCustomers_DefaultLimit_ReturnsSuccessOrNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/reports/customers/top");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTopCustomers_CustomLimit_ReturnsSuccessOrNotFound()
    {
        // Arrange
        var limit = 5;

        // Act
        var response = await _client.GetAsync($"/api/reports/customers/top?limit={limit}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTopRewards_DefaultLimit_ReturnsSuccessOrNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/reports/rewards/top");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTopRewards_CustomLimit_ReturnsSuccessOrNotFound()
    {
        // Arrange
        var limit = 3;

        // Act
        var response = await _client.GetAsync($"/api/reports/rewards/top?limit={limit}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPointsSummary_ReturnsSuccessOrNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/reports/points/summary");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }
}
