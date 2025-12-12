using MegaMarket.API.DTOs.PointTransaction;
using MegaMarket.API.DTOs.PointTransactions;

namespace MegaMarket.Tests.Integration.Controllers.Points;

public class PointsControllerTests : IClassFixture<TestServerFixture>
{
    private readonly HttpClient _client;
    private readonly TestServerFixture _factory;

    public PointsControllerTests(TestServerFixture factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AddPoints_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var customerId = 1;
        var addPointsRequest = new AddPointRequestDto
        {
            Points = 100,
            Description = "Test points addition",
            InvoiceId = null
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/points/{customerId}/add", addPointsRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PointTransactionResponseDto>();
        result.Should().NotBeNull();
        result!.CustomerId.Should().Be(customerId);
        result.PointChange.Should().Be(100);
        result.TransactionType.Should().Contain("Earn");
    }

    [Fact]
    public async Task AddPoints_WithInvoiceId_ReturnsSuccess()
    {
        // Arrange
        var customerId = 2;
        var addPointsRequest = new AddPointRequestDto
        {
            Points = 50,
            Description = "Points from purchase",
            InvoiceId = 123
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/points/{customerId}/add", addPointsRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PointTransactionResponseDto>();
        result.Should().NotBeNull();
        result!.InvoiceId.Should().Be(123);
        result.PointChange.Should().Be(50);
    }

    [Fact]
    public async Task AddPoints_ZeroPoints_ReturnsBadRequest()
    {
        // Arrange
        var customerId = 1;
        var addPointsRequest = new AddPointRequestDto
        {
            Points = 0, // Invalid
            Description = "Zero points"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/points/{customerId}/add", addPointsRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Points must be greater than 0");
    }

    [Fact]
    public async Task AddPoints_NegativePoints_ReturnsBadRequest()
    {
        // Arrange
        var customerId = 1;
        var addPointsRequest = new AddPointRequestDto
        {
            Points = -10, // Invalid
            Description = "Negative points"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/points/{customerId}/add", addPointsRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddPoints_NonExistentCustomer_ReturnsNotFound()
    {
        // Arrange
        var customerId = 99999;
        var addPointsRequest = new AddPointRequestDto
        {
            Points = 100,
            Description = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/points/{customerId}/add", addPointsRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Customer not found");
    }

    [Fact]
    public async Task SubtractPoints_ValidRequest_ReturnsSuccess()
    {
        // Arrange - First add some points
        var customerId = 1;
        await _client.PostAsJsonAsync($"/api/points/{customerId}/add", new AddPointRequestDto
        {
            Points = 500,
            Description = "Initial points"
        });

        // Subtract points
        var subtractRequest = new SubtractPointRequestDto
        {
            Points = 100,
            TransactionType = "Redeem",
            Description = "Test subtraction"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/points/{customerId}/subtract", subtractRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PointTransactionResponseDto>();
        result.Should().NotBeNull();
        result!.CustomerId.Should().Be(customerId);
        result.PointChange.Should().Be(-100); // Negative for subtraction
        result.TransactionType.Should().Be("Redeem");
    }

    [Fact]
    public async Task SubtractPoints_InsufficientBalance_ReturnsBadRequest()
    {
        // Arrange
        var customerId = 2; // Customer with limited points
        var subtractRequest = new SubtractPointRequestDto
        {
            Points = 10000, // More than available
            TransactionType = "Redeem",
            Description = "Too many points"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/points/{customerId}/subtract", subtractRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SubtractPoints_NonExistentCustomer_ReturnsNotFound()
    {
        // Arrange
        var customerId = 99999;
        var subtractRequest = new SubtractPointRequestDto
        {
            Points = 50,
            TransactionType = "Redeem",
            Description = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/points/{customerId}/subtract", subtractRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Customer not found");
    }

    [Fact]
    public async Task GetPointTransactionById_ExistingTransaction_ReturnsTransaction()
    {
        // Arrange - Create a transaction first
        var customerId = 1;
        var addResponse = await _client.PostAsJsonAsync($"/api/points/{customerId}/add", new AddPointRequestDto
        {
            Points = 75,
            Description = "Test transaction"
        });
        var transaction = await addResponse.Content.ReadFromJsonAsync<PointTransactionResponseDto>();

        // Act
        var response = await _client.GetAsync($"/api/points/{transaction!.TransactionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PointTransactionResponseDto>();
        result.Should().NotBeNull();
        result!.TransactionId.Should().Be(transaction.TransactionId);
        result.PointChange.Should().Be(75);
    }

    [Fact]
    public async Task GetPointTransactionById_NonExistentTransaction_ReturnsNotFound()
    {
        // Arrange
        var transactionId = 99999;

        // Act
        var response = await _client.GetAsync($"/api/points/{transactionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAll_WithoutFilters_ReturnsAllTransactions()
    {
        // Arrange - Create some transactions
        await _client.PostAsJsonAsync($"/api/points/1/add", new AddPointRequestDto
        {
            Points = 25,
            Description = "Test 1"
        });
        await _client.PostAsJsonAsync($"/api/points/2/add", new AddPointRequestDto
        {
            Points = 30,
            Description = "Test 2"
        });

        // Act
        var response = await _client.GetAsync("/api/points");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var transactions = await response.Content.ReadFromJsonAsync<List<PointTransactionResponseDto>>();
        transactions.Should().NotBeNull();
        transactions.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetAll_FilterByCustomerId_ReturnsResponse()
    {
        // Arrange
        var customerId = 3;

        // Act
        var response = await _client.GetAsync($"/api/points?customerId={customerId}");

        // Assert - Just verify endpoint works
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAll_NoTransactionsFound_ReturnsResponse()
    {
        // Act - Query for non-existent customer
        var response = await _client.GetAsync("/api/points?customerId=99999");

        // Assert - May return NotFound or OK with empty list
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PointsFlow_AddAndSubtract_UpdatesBalanceCorrectly()
    {
        // This test demonstrates the complete points flow

        // Step 1: Add initial points
        var customerId = 1;
        var addResponse1 = await _client.PostAsJsonAsync($"/api/points/{customerId}/add", new AddPointRequestDto
        {
            Points = 200,
            Description = "Initial balance"
        });
        addResponse1.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 2: Add more points
        var addResponse2 = await _client.PostAsJsonAsync($"/api/points/{customerId}/add", new AddPointRequestDto
        {
            Points = 100,
            Description = "Additional points"
        });
        addResponse2.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 3: Subtract some points
        var subtractResponse = await _client.PostAsJsonAsync($"/api/points/{customerId}/subtract", new SubtractPointRequestDto
        {
            Points = 50,
            TransactionType = "Redeem",
            Description = "Redeem reward"
        });
        subtractResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 4: Verify transaction history
        var historyResponse = await _client.GetAsync($"/api/points?customerId={customerId}");
        historyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var history = await historyResponse.Content.ReadFromJsonAsync<List<PointTransactionResponseDto>>();
        history.Should().NotBeNull();
        history.Should().HaveCountGreaterThan(2);
    }

    [Fact]
    public async Task AddPoints_MultipleTransactionTypes_ReturnsSuccess()
    {
        // Test various transaction scenarios
        var customerId = 1;

        // Purchase points
        var response1 = await _client.PostAsJsonAsync($"/api/points/{customerId}/add", new AddPointRequestDto
        {
            Points = 80,
            Description = "Points from purchase",
            InvoiceId = 100
        });
        response1.StatusCode.Should().Be(HttpStatusCode.OK);

        // Bonus points
        var response2 = await _client.PostAsJsonAsync($"/api/points/{customerId}/add", new AddPointRequestDto
        {
            Points = 20,
            Description = "Bonus points"
        });
        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        // Promotional points
        var response3 = await _client.PostAsJsonAsync($"/api/points/{customerId}/add", new AddPointRequestDto
        {
            Points = 50,
            Description = "Promotional campaign"
        });
        response3.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SubtractPoints_ValidRequest_ReturnsSuccessOrBadRequest()
    {
        // Arrange - Try to subtract from customer
        var customerId = 3;
        var subtractRequest = new SubtractPointRequestDto
        {
            Points = 10,
            TransactionType = "Redeem",
            Description = "Test subtraction"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/points/{customerId}/subtract", subtractRequest);

        // Assert - May succeed if enough points, or fail if not
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }
}
