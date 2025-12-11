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
    public async Task SaveInvoice_ValidData_ReturnsSuccess()
    {
        // Arrange
        var invoice = new
        {
            CustomerId = 1,
            InvoiceDate = DateTime.Now,
            TotalAmount = 150000,
            PaymentMethod = "Cash",
            Status = "Completed",
            InvoiceDetails = new[]
            {
                new
                {
                    ProductId = 1,
                    Quantity = 2,
                    UnitPrice = 45000,
                    Subtotal = 90000
                },
                new
                {
                    ProductId = 2,
                    Quantity = 3,
                    UnitPrice = 18000,
                    Subtotal = 54000
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/invoice", invoice);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SaveInvoice_SingleItem_ReturnsSuccess()
    {
        // Arrange
        var invoice = new
        {
            CustomerId = 2,
            InvoiceDate = DateTime.Now,
            TotalAmount = 52000,
            PaymentMethod = "Card",
            Status = "Completed",
            InvoiceDetails = new[]
            {
                new
                {
                    ProductId = 3,
                    Quantity = 1,
                    UnitPrice = 52000,
                    Subtotal = 52000
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/invoice", invoice);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SaveInvoice_WithDiscount_ReturnsSuccess()
    {
        // Arrange
        var invoice = new
        {
            CustomerId = 3,
            InvoiceDate = DateTime.Now,
            TotalAmount = 85500,
            DiscountAmount = 9500,
            OriginalAmount = 95000,
            PaymentMethod = "Cash",
            Status = "Completed",
            InvoiceDetails = new[]
            {
                new
                {
                    ProductId = 4,
                    Quantity = 1,
                    UnitPrice = 95000,
                    Subtotal = 95000
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/invoice", invoice);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SaveInvoice_MultipleProducts_CalculatesTotalCorrectly()
    {
        // Arrange
        var invoice = new
        {
            CustomerId = 1,
            InvoiceDate = DateTime.Now,
            TotalAmount = 310000,
            PaymentMethod = "Cash",
            Status = "Completed",
            InvoiceDetails = new[]
            {
                new { ProductId = 1, Quantity = 2, UnitPrice = 45000, Subtotal = 90000 },
                new { ProductId = 2, Quantity = 5, UnitPrice = 18000, Subtotal = 90000 },
                new { ProductId = 4, Quantity = 1, UnitPrice = 95000, Subtotal = 95000 },
                new { ProductId = 3, Quantity = 1, UnitPrice = 35000, Subtotal = 35000 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/invoice", invoice);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SaveInvoice_DifferentPaymentMethods_ReturnsSuccess()
    {
        // Test various payment methods
        var paymentMethods = new[] { "Cash", "Card", "Transfer", "E-Wallet" };

        foreach (var method in paymentMethods)
        {
            // Arrange
            var invoice = new
            {
                CustomerId = 1,
                InvoiceDate = DateTime.Now,
                TotalAmount = 50000,
                PaymentMethod = method,
                Status = "Completed",
                InvoiceDetails = new[]
                {
                    new { ProductId = 1, Quantity = 1, UnitPrice = 45000, Subtotal = 45000 }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/invoice", invoice);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    [Fact]
    public async Task SaveInvoice_WithPointsRedemption_ReturnsSuccess()
    {
        // Arrange - Invoice with points redeemed
        var invoice = new
        {
            CustomerId = 3,
            InvoiceDate = DateTime.Now,
            TotalAmount = 100000,
            PointsEarned = 10,
            PointsRedeemed = 50,
            PaymentMethod = "Cash",
            Status = "Completed",
            InvoiceDetails = new[]
            {
                new { ProductId = 1, Quantity = 2, UnitPrice = 50000, Subtotal = 100000 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/invoice", invoice);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SaveInvoice_PendingStatus_ReturnsSuccess()
    {
        // Arrange - Invoice with pending status
        var invoice = new
        {
            CustomerId = 2,
            InvoiceDate = DateTime.Now,
            TotalAmount = 75000,
            PaymentMethod = "Card",
            Status = "Pending",
            InvoiceDetails = new[]
            {
                new { ProductId = 2, Quantity = 4, UnitPrice = 18000, Subtotal = 72000 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/invoice", invoice);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SaveInvoice_LargeOrder_ReturnsSuccess()
    {
        // Arrange - Large order with multiple items
        var invoice = new
        {
            CustomerId = 1,
            InvoiceDate = DateTime.Now,
            TotalAmount = 1500000,
            PaymentMethod = "Transfer",
            Status = "Completed",
            InvoiceDetails = new[]
            {
                new { ProductId = 1, Quantity = 10, UnitPrice = 45000, Subtotal = 450000 },
                new { ProductId = 2, Quantity = 20, UnitPrice = 18000, Subtotal = 360000 },
                new { ProductId = 3, Quantity = 5, UnitPrice = 52000, Subtotal = 260000 },
                new { ProductId = 4, Quantity = 4, UnitPrice = 95000, Subtotal = 380000 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/invoice", invoice);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
