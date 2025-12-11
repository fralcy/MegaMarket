using MegaMarket.API.DTOs.CustomerRewards;
using MegaMarket.API.DTOs.Customers;
using MegaMarket.API.DTOs.Rewards;

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
    public async Task RedeemReward_ValidRequest_ReturnsSuccess()
    {
        // Arrange - Customer with enough points
        var customerId = 3; // Le Van C has 2000 points
        var rewardId = 1;   // 10% Discount Voucher costs 100 points

        var redeemRequest = new RedeemRewardRequestDto
        {
            CustomerId = customerId,
            RewardId = rewardId,
            InvoiceId = null
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/customerrewards/redeem", redeemRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Reward redeemed successfully");
    }

    [Fact]
    public async Task RedeemReward_InsufficientPoints_ReturnsBadRequest()
    {
        // Arrange - Customer with insufficient points
        var customerId = 2; // Tran Thi B has 500 points
        var rewardId = 3;   // 50k Cash Voucher costs 500 points (exactly enough)

        // First redeem to use all points
        await _client.PostAsJsonAsync("/api/customerrewards/redeem", new RedeemRewardRequestDto
        {
            CustomerId = customerId,
            RewardId = rewardId
        });

        // Try to redeem again with no points left
        var redeemRequest = new RedeemRewardRequestDto
        {
            CustomerId = customerId,
            RewardId = rewardId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/customerrewards/redeem", redeemRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RedeemReward_InvalidCustomerId_ReturnsBadRequest()
    {
        // Arrange
        var redeemRequest = new RedeemRewardRequestDto
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
        var redeemRequest = new RedeemRewardRequestDto
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
    public async Task GetAll_WithoutFilters_ReturnsAllCustomerRewards()
    {
        // Arrange - First redeem a reward to have data
        await _client.PostAsJsonAsync("/api/customerrewards/redeem", new RedeemRewardRequestDto
        {
            CustomerId = 1,
            RewardId = 1
        });

        // Act
        var response = await _client.GetAsync("/api/customerrewards");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var rewards = await response.Content.ReadFromJsonAsync<List<CustomerRewardResponseDto>>();
        rewards.Should().NotBeNull();
        rewards.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetAll_FilterByStatus_ReturnsFilteredResults()
    {
        // Arrange - Redeem a reward
        await _client.PostAsJsonAsync("/api/customerrewards/redeem", new RedeemRewardRequestDto
        {
            CustomerId = 1,
            RewardId = 2
        });

        // Act
        var response = await _client.GetAsync("/api/customerrewards?status=Pending");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var rewards = await response.Content.ReadFromJsonAsync<List<CustomerRewardResponseDto>>();
        rewards.Should().NotBeNull();
        rewards.Should().OnlyContain(r => r.Status == "Pending");
    }

    [Fact]
    public async Task GetAll_FilterByCustomerId_ReturnsCustomerRewards()
    {
        // Arrange - Redeem rewards for specific customer
        var customerId = 1;
        await _client.PostAsJsonAsync("/api/customerrewards/redeem", new RedeemRewardRequestDto
        {
            CustomerId = customerId,
            RewardId = 1
        });

        // Act
        var response = await _client.GetAsync($"/api/customerrewards?customerId={customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var rewards = await response.Content.ReadFromJsonAsync<List<CustomerRewardResponseDto>>();
        rewards.Should().NotBeNull();
        rewards.Should().OnlyContain(r => r.CustomerId == customerId);
    }

    [Fact]
    public async Task GetAll_NoRewardsFound_ReturnsNotFound()
    {
        // Act - Query for non-existent status
        var response = await _client.GetAsync("/api/customerrewards?status=NonExistentStatus");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByCustomer_ExistingCustomerWithRewards_ReturnsRewards()
    {
        // Arrange - Redeem a reward for customer
        var customerId = 1;
        await _client.PostAsJsonAsync("/api/customerrewards/redeem", new RedeemRewardRequestDto
        {
            CustomerId = customerId,
            RewardId = 1
        });

        // Act
        var response = await _client.GetAsync($"/api/customerrewards/customer/{customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var rewards = await response.Content.ReadFromJsonAsync<List<CustomerRewardResponseDto>>();
        rewards.Should().NotBeNull();
        rewards.Should().HaveCountGreaterThan(0);
        rewards.Should().OnlyContain(r => r.CustomerId == customerId);
    }

    [Fact]
    public async Task GetByCustomer_CustomerWithNoRewards_ReturnsNotFound()
    {
        // Arrange - Customer with no rewards (using high ID that likely has no rewards)
        var customerId = 2;

        // Act
        var response = await _client.GetAsync($"/api/customerrewards/customer/{customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ClaimReward_PendingReward_UpdatesStatusToClaimed()
    {
        // Arrange - Redeem a voucher reward
        var redeemResponse = await _client.PostAsJsonAsync("/api/customerrewards/redeem", new RedeemRewardRequestDto
        {
            CustomerId = 1,
            RewardId = 1 // Voucher type
        });
        var redeemContent = await redeemResponse.Content.ReadFromJsonAsync<dynamic>();
        var redemptionId = (int)redeemContent!.data.redemptionId;

        // Act
        var response = await _client.PutAsync($"/api/customerrewards/{redemptionId}/claim", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Reward claimed successfully");
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
    public async Task UseReward_ClaimedReward_UpdatesStatusToUsed()
    {
        // Arrange - Redeem and claim a reward
        var redeemResponse = await _client.PostAsJsonAsync("/api/customerrewards/redeem", new RedeemRewardRequestDto
        {
            CustomerId = 1,
            RewardId = 2
        });
        var redeemContent = await redeemResponse.Content.ReadFromJsonAsync<dynamic>();
        var redemptionId = (int)redeemContent!.data.redemptionId;

        // Claim the reward
        await _client.PutAsync($"/api/customerrewards/{redemptionId}/claim", null);

        // Act - Use the reward
        var response = await _client.PutAsJsonAsync($"/api/customerrewards/{redemptionId}/use", new { });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Reward used successfully");
    }

    [Fact]
    public async Task ApplyVoucherToInvoice_ValidRequest_UpdatesRewardWithInvoice()
    {
        // Arrange - Redeem and claim a voucher
        var redeemResponse = await _client.PostAsJsonAsync("/api/customerrewards/redeem", new RedeemRewardRequestDto
        {
            CustomerId = 3,
            RewardId = 1
        });
        var redeemContent = await redeemResponse.Content.ReadFromJsonAsync<dynamic>();
        var redemptionId = (int)redeemContent!.data.redemptionId;

        // Claim the voucher
        await _client.PutAsync($"/api/customerrewards/{redemptionId}/claim", null);

        // Act - Apply to invoice
        var applyRequest = new { InvoiceId = 123 };
        var response = await _client.PutAsJsonAsync($"/api/customerrewards/{redemptionId}/apply-to-invoice", applyRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Voucher applied to invoice successfully");
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

    [Fact]
    public async Task RewardWorkflow_CompleteFlow_Success()
    {
        // This test demonstrates the complete reward redemption workflow

        // Step 1: Redeem a reward
        var redeemResponse = await _client.PostAsJsonAsync("/api/customerrewards/redeem", new RedeemRewardRequestDto
        {
            CustomerId = 3,
            RewardId = 2
        });
        redeemResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var redeemContent = await redeemResponse.Content.ReadFromJsonAsync<dynamic>();
        var redemptionId = (int)redeemContent!.data.redemptionId;

        // Step 2: Verify reward is in Pending status
        var getResponse = await _client.GetAsync($"/api/customerrewards/customer/3");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 3: Claim the reward
        var claimResponse = await _client.PutAsync($"/api/customerrewards/{redemptionId}/claim", null);
        claimResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 4: Apply to invoice
        var applyResponse = await _client.PutAsJsonAsync($"/api/customerrewards/{redemptionId}/apply-to-invoice", new { InvoiceId = 456 });
        applyResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 5: Verify final state
        var finalGetResponse = await _client.GetAsync($"/api/customerrewards/customer/3");
        finalGetResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var finalRewards = await finalGetResponse.Content.ReadFromJsonAsync<List<CustomerRewardResponseDto>>();
        finalRewards.Should().Contain(r => r.RedemptionId == redemptionId && r.Status == "Used");
    }
}
