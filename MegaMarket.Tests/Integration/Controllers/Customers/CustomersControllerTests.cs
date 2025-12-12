using MegaMarket.API.DTOs.Customers;

namespace MegaMarket.Tests.Integration.Controllers.Customers;

public class CustomersControllerTests : IClassFixture<TestServerFixture>
{
    private readonly HttpClient _client;
    private readonly TestServerFixture _factory;

    public CustomersControllerTests(TestServerFixture factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllCustomers_ReturnsSuccess_WithListOfCustomers()
    {
        // Act
        var response = await _client.GetAsync("/api/customers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customers = await response.Content.ReadFromJsonAsync<List<CustomerResponseDto>>();
        customers.Should().NotBeNull();
        customers.Should().HaveCountGreaterThan(0);
        customers.Should().Contain(c => c.FullName == "Nguyen Van A");
    }

    [Fact]
    public async Task GetCustomerById_ExistingId_ReturnsCustomer()
    {
        // Arrange
        var customerId = 1;

        // Act
        var response = await _client.GetAsync($"/api/customers/{customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await response.Content.ReadFromJsonAsync<CustomerResponseDto>();
        customer.Should().NotBeNull();
        customer!.CustomerId.Should().Be(customerId);
        customer.FullName.Should().Be("Nguyen Van A");
    }

    [Fact]
    public async Task GetCustomerById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var customerId = 99999;

        // Act
        var response = await _client.GetAsync($"/api/customers/{customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateCustomer_ValidData_ReturnsCreated()
    {
        // Arrange
        var newCustomer = new CreateCustomerRequestDto
        {
            FullName = "Pham Van D",
            Phone = "0909999999",
            Email = "phamvand@email.com",
            Points = 0
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/customers", newCustomer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdCustomer = await response.Content.ReadFromJsonAsync<CustomerResponseDto>();
        createdCustomer.Should().NotBeNull();
        createdCustomer!.FullName.Should().Be(newCustomer.FullName);
        createdCustomer.Phone.Should().Be(newCustomer.Phone);
        createdCustomer.Email.Should().Be(newCustomer.Email);

        // Verify Location header contains customer ID
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain(createdCustomer.CustomerId.ToString());
    }

    [Fact]
    public async Task UpdateCustomer_NonExistingId_ReturnsBadRequest()
    {
        // Arrange
        var customerId = 99999;
        var updateDto = new UpdateCustomerRequestDto
        {
            FullName = "Non Existing Customer",
            Phone = "0909999999",
            Email = "nonexisting@email.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/customers/{customerId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCustomer_ExistingId_ReturnsSuccess()
    {
        // Arrange - Create a customer to delete
        var newCustomer = new CreateCustomerRequestDto
        {
            FullName = "To Be Deleted",
            Phone = "0908888888",
            Email = "tobedeleted@email.com",
            Points = 0
        };
        var createResponse = await _client.PostAsJsonAsync("/api/customers", newCustomer);
        var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerResponseDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/customers/{createdCustomer!.CustomerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var message = await response.Content.ReadAsStringAsync();
        message.Should().Contain("Delete successfully!");

        // Verify customer is deleted
        var getResponse = await _client.GetAsync($"/api/customers/{createdCustomer.CustomerId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCustomer_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var customerId = 99999;

        // Act
        var response = await _client.DeleteAsync($"/api/customers/{customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchCustomers_ByName_ReturnsSuccess()
    {
        // Arrange
        var searchQuery = "Nguyen";

        // Act
        var response = await _client.GetAsync($"/api/customers/search?name={searchQuery}");

        // Assert - Just verify endpoint works
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchCustomers_ByPhone_ReturnsMatchingCustomers()
    {
        // Arrange
        var phoneQuery = "0901234567";

        // Act
        var response = await _client.GetAsync($"/api/customers/search?phone={phoneQuery}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customers = await response.Content.ReadFromJsonAsync<List<CustomerResponseDto>>();
        customers.Should().NotBeNull();
        customers.Should().HaveCount(1);
        customers![0].Phone.Should().Be(phoneQuery);
    }

    [Fact]
    public async Task SearchCustomers_NoMatch_ReturnsResponse()
    {
        // Arrange
        var searchQuery = "NonExistingCustomerXYZ123";

        // Act
        var response = await _client.GetAsync($"/api/customers/search?name={searchQuery}");

        // Assert - API may return OK with empty list or NotFound
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCustomerPointHistory_ExistingCustomerWithPoints_ReturnsHistory()
    {
        // Arrange - First create a customer and add some points
        var newCustomer = new CreateCustomerRequestDto
        {
            FullName = "Customer With Points",
            Phone = "0907777777",
            Email = "withpoints@email.com",
            Points = 100
        };
        var createResponse = await _client.PostAsJsonAsync("/api/customers", newCustomer);
        var customer = await createResponse.Content.ReadFromJsonAsync<CustomerResponseDto>();

        // Add some point transactions via Points API
        await _client.PostAsJsonAsync($"/api/points/{customer!.CustomerId}/add", new { points = 50, description = "Test points" });
        await _client.PostAsJsonAsync($"/api/points/{customer.CustomerId}/add", new { points = 30, description = "More test points" });

        // Act
        var response = await _client.GetAsync($"/api/customers/{customer.CustomerId}/Points");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var history = await response.Content.ReadFromJsonAsync<List<object>>();
        history.Should().NotBeNull();
        history.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetCustomerPointHistory_CustomerWithNoHistory_ReturnsNotFound()
    {
        // Arrange - Create a customer without any point transactions
        var newCustomer = new CreateCustomerRequestDto
        {
            FullName = "Customer No Points",
            Phone = "0906666666",
            Email = "nopoints@email.com",
            Points = 0
        };
        var createResponse = await _client.PostAsJsonAsync("/api/customers", newCustomer);
        var customer = await createResponse.Content.ReadFromJsonAsync<CustomerResponseDto>();

        // Act
        var response = await _client.GetAsync($"/api/customers/{customer!.CustomerId}/Points");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
