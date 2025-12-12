using MegaMarket.API.DTOs.Rewards;

namespace MegaMarket.Tests.Integration.Controllers.Rewards;

public class RewardsControllerTests : IClassFixture<TestServerFixture>
{
    private readonly HttpClient _client;
    private readonly TestServerFixture _factory;

    public RewardsControllerTests(TestServerFixture factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllRewards_ReturnsSuccess_WithListOfRewards()
    {
        // Act
        var response = await _client.GetAsync("/api/rewards");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var rewards = await response.Content.ReadFromJsonAsync<List<RewardResponseDto>>();
        rewards.Should().NotBeNull();
        rewards.Should().HaveCountGreaterThan(0);
        rewards.Should().Contain(r => r.Name == "10% Discount Voucher");
    }

    [Fact]
    public async Task GetRewardById_ExistingId_ReturnsReward()
    {
        // Arrange
        var rewardId = 1;

        // Act
        var response = await _client.GetAsync($"/api/rewards/{rewardId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var reward = await response.Content.ReadFromJsonAsync<RewardResponseDto>();
        reward.Should().NotBeNull();
        reward!.RewardId.Should().Be(rewardId);
        reward.Name.Should().Be("10% Discount Voucher");
        reward.PointCost.Should().Be(100);
    }

    [Fact]
    public async Task GetRewardById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var rewardId = 99999;

        // Act
        var response = await _client.GetAsync($"/api/rewards/{rewardId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateReward_ValidData_ReturnsCreated()
    {
        // Arrange
        var newReward = new CreateRewardRequestDto
        {
            Name = "100k Cash Voucher",
            Description = "100,000 VND discount voucher",
            PointCost = 1000,
            RewardType = "Voucher",
            Value = 100000,
            QuantityAvailable = 50
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/rewards", newReward);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdReward = await response.Content.ReadFromJsonAsync<RewardResponseDto>();
        createdReward.Should().NotBeNull();
        createdReward!.Name.Should().Be(newReward.Name);
        createdReward.PointCost.Should().Be(newReward.PointCost);
        createdReward.RewardType.Should().Be(newReward.RewardType);
        createdReward.Value.Should().Be(newReward.Value);
        createdReward.IsActive.Should().BeTrue();

        // Verify Location header contains reward ID
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain(createdReward.RewardId.ToString());
    }

    [Fact]
    public async Task CreateReward_FreeGiftType_ReturnsCreated()
    {
        // Arrange
        var newReward = new CreateRewardRequestDto
        {
            Name = "Free Coffee Mug",
            Description = "Free branded coffee mug",
            PointCost = 300,
            RewardType = "Gift",
            Value = 0,
            QuantityAvailable = 100
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/rewards", newReward);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdReward = await response.Content.ReadFromJsonAsync<RewardResponseDto>();
        createdReward.Should().NotBeNull();
        createdReward!.RewardType.Should().Be("Gift");
        createdReward.Value.Should().Be(0);
    }

    [Fact]
    public async Task CreateReward_InvalidPointCost_ReturnsBadRequest()
    {
        // Arrange - PointCost must be at least 1
        var invalidReward = new CreateRewardRequestDto
        {
            Name = "Invalid Reward",
            Description = "This should fail",
            PointCost = 0, // Invalid: must be >= 1
            RewardType = "Voucher",
            Value = 10000,
            QuantityAvailable = 10
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/rewards", invalidReward);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateReward_ValidData_ReturnsSuccess()
    {
        // Arrange - First create a reward to update
        var createDto = new CreateRewardRequestDto
        {
            Name = "Reward To Update",
            Description = "Original description",
            PointCost = 500,
            RewardType = "Voucher",
            Value = 50000,
            QuantityAvailable = 30
        };
        var createResponse = await _client.PostAsJsonAsync("/api/rewards", createDto);
        var createdReward = await createResponse.Content.ReadFromJsonAsync<RewardResponseDto>();

        // Update the reward
        var updateDto = new UpdateRewardRequestDto
        {
            Name = "Reward Updated",
            Description = "Updated description",
            PointCost = 600,
            RewardType = "Voucher",
            Value = 60000,
            QuantityAvailable = 40
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/rewards/{createdReward!.RewardId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedReward = await response.Content.ReadFromJsonAsync<RewardResponseDto>();
        updatedReward.Should().NotBeNull();
        updatedReward!.Name.Should().Be(updateDto.Name);
        updatedReward.Description.Should().Be(updateDto.Description);
        updatedReward.PointCost.Should().Be(updateDto.PointCost);
        updatedReward.Value.Should().Be(updateDto.Value);
    }

    [Fact]
    public async Task UpdateReward_NonExistingId_ReturnsBadRequest()
    {
        // Arrange
        var rewardId = 99999;
        var updateDto = new UpdateRewardRequestDto
        {
            Name = "Non Existing Reward",
            Description = "This should fail",
            PointCost = 100,
            RewardType = "Voucher",
            Value = 10000,
            QuantityAvailable = 10
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/rewards/{rewardId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteReward_ExistingId_ReturnsNoContent()
    {
        // Arrange - Create a reward to delete (soft delete: sets IsActive = false)
        var createDto = new CreateRewardRequestDto
        {
            Name = "Reward To Delete",
            Description = "Will be soft deleted",
            PointCost = 200,
            RewardType = "Voucher",
            Value = 20000,
            QuantityAvailable = 20
        };
        var createResponse = await _client.PostAsJsonAsync("/api/rewards", createDto);
        var createdReward = await createResponse.Content.ReadFromJsonAsync<RewardResponseDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/rewards/{createdReward!.RewardId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify reward is soft deleted (IsActive = false)
        var getResponse = await _client.GetAsync($"/api/rewards/{createdReward.RewardId}");
        if (getResponse.StatusCode == HttpStatusCode.OK)
        {
            var deletedReward = await getResponse.Content.ReadFromJsonAsync<RewardResponseDto>();
            deletedReward!.IsActive.Should().BeFalse();
        }
    }

    [Fact]
    public async Task DeleteReward_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var rewardId = 99999;

        // Act
        var response = await _client.DeleteAsync($"/api/rewards/{rewardId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllRewards_VerifySeededRewards_Present()
    {
        // Act
        var response = await _client.GetAsync("/api/rewards");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var rewards = await response.Content.ReadFromJsonAsync<List<RewardResponseDto>>();
        rewards.Should().NotBeNull();

        // Verify seeded rewards from TestData
        rewards.Should().Contain(r => r.Name == "10% Discount Voucher" && r.PointCost == 100);
        rewards.Should().Contain(r => r.Name == "Free Shipping Voucher" && r.PointCost == 200);
        rewards.Should().Contain(r => r.Name == "50k Cash Voucher" && r.PointCost == 500);
    }

    [Fact]
    public async Task CreateReward_MultipleRewardTypes_ReturnsCreated()
    {
        // Test creating different reward types
        var rewardTypes = new[]
        {
            new CreateRewardRequestDto
            {
                Name = "Percentage Discount",
                Description = "20% off",
                PointCost = 200,
                RewardType = "Percentage",
                Value = 20,
                QuantityAvailable = 100
            },
            new CreateRewardRequestDto
            {
                Name = "Fixed Amount Discount",
                Description = "75k off",
                PointCost = 750,
                RewardType = "FixedAmount",
                Value = 75000,
                QuantityAvailable = 50
            },
            new CreateRewardRequestDto
            {
                Name = "Free Product",
                Description = "Free item",
                PointCost = 400,
                RewardType = "Product",
                Value = 0,
                QuantityAvailable = 30
            }
        };

        // Act & Assert
        foreach (var rewardDto in rewardTypes)
        {
            var response = await _client.PostAsJsonAsync("/api/rewards", rewardDto);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await response.Content.ReadFromJsonAsync<RewardResponseDto>();
            created.Should().NotBeNull();
            created!.RewardType.Should().Be(rewardDto.RewardType);
        }
    }

    [Fact]
    public async Task UpdateReward_UpdateQuantityOnly_ReturnsSuccess()
    {
        // Arrange - Create a reward
        var createDto = new CreateRewardRequestDto
        {
            Name = "Limited Reward",
            Description = "Limited quantity",
            PointCost = 150,
            RewardType = "Voucher",
            Value = 15000,
            QuantityAvailable = 10
        };
        var createResponse = await _client.PostAsJsonAsync("/api/rewards", createDto);
        var createdReward = await createResponse.Content.ReadFromJsonAsync<RewardResponseDto>();

        // Update only quantity
        var updateDto = new UpdateRewardRequestDto
        {
            Name = createdReward!.Name,
            Description = createdReward.Description,
            PointCost = createdReward.PointCost,
            RewardType = createdReward.RewardType,
            Value = createdReward.Value,
            QuantityAvailable = 50 // Increased quantity
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/rewards/{createdReward.RewardId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedReward = await response.Content.ReadFromJsonAsync<RewardResponseDto>();
        updatedReward!.QuantityAvailable.Should().Be(50);
    }
}
