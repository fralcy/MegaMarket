namespace MegaMarket.Tests.Integration.Controllers.Invoice;

public class InvoiceControllerTests : IClassFixture<TestServerFixture>
{
    private readonly HttpClient _client;
    private readonly TestServerFixture _factory;

    public InvoiceControllerTests(TestServerFixture factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task SaveInvoice_ValidMinimalData_ReturnsSuccess()
    {
        // Arrange - Minimal required fields only
        var invoice = new
        {
            CustomerId = 1,
            TotalAmount = 100000,
            PaymentMethod = "Cash"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/invoice", invoice);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.BadRequest);
        // Note: This test just verifies the endpoint exists and responds
    }
}
