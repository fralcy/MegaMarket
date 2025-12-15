using MegaMarket.API.DTOs.CustomerRewards;

namespace MegaMarket.Tests.Integration.Controllers.CustomerRewards;

public class CustomerRewardsControllerTests : IClassFixture<TestServerFixture>
{
    private readonly HttpClient _client;
    private readonly TestServerFixture _factory;

    public CustomerRewardsControllerTests(TestServerFixture factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_NoTransactionsFound_ReturnsNotFound()
    {
        // Act - Query for non-existent customer rewards
        var response = await _client.GetAsync("/api/customerrewards?customerId=99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByCustomer_CustomerWithNoRewards_ReturnsNotFound()
    {
        // Arrange - Customer with no rewards
        var customerId = 2;

        // Act
        var response = await _client.GetAsync($"/api/customerrewards/customer/{customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RedeemReward_InvalidCustomerId_ReturnsBadRequest()
    {
        // Arrange
        var redeemRequest = new
        {
            CustomerId = 0, // Invalid
            RewardId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/customerrewards/redeem", redeemRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid customer ID");
    }

    [Fact]
    public async Task RedeemReward_InvalidRewardId_ReturnsBadRequest()
    {
        // Arrange
        var redeemRequest = new
        {
            CustomerId = 1,
            RewardId = 0 // Invalid
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/customerrewards/redeem", redeemRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid reward ID");
    }

    [Fact]
    public async Task ClaimReward_NonExistentReward_ReturnsNotFound()
    {
        // Arrange
        var redemptionId = 99999;

        // Act
        var response = await _client.PutAsync($"/api/customerrewards/{redemptionId}/claim", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ApplyVoucherToInvoice_InvalidInvoiceId_ReturnsBadRequest()
    {
        // Arrange
        var redemptionId = 1;
        var applyRequest = new { InvoiceId = 0 }; // Invalid

        // Act
        var response = await _client.PutAsJsonAsync($"/api/customerrewards/{redemptionId}/apply-to-invoice", applyRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid invoice ID");
    }

    [Fact]
    public async Task ApplyVoucherToInvoice_NonExistentRedemption_ReturnsNotFound()
    {
        // Arrange
        var redemptionId = 99999;
        var applyRequest = new { InvoiceId = 123 };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/customerrewards/{redemptionId}/apply-to-invoice", applyRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
