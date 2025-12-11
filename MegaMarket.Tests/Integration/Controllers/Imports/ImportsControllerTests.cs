using MegaMarket.API.DTOs.Imports;

namespace MegaMarket.Tests.Integration.Controllers.Imports;

public class ImportsControllerTests : IClassFixture<TestServerFixture>
{
    private readonly HttpClient _client;
    private readonly TestServerFixture _factory;

    public ImportsControllerTests(TestServerFixture factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetImports_ReturnsSuccess_WithListOfImports()
    {
        // Act
        var response = await _client.GetAsync("/api/imports");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var imports = await response.Content.ReadFromJsonAsync<List<ImportSummaryDto>>();
        imports.Should().NotBeNull();
    }

    [Fact]
    public async Task GetImport_ExistingId_ReturnsImportWithDetails()
    {
        // Arrange - First create an import
        var createDto = new
        {
            Supplier = "Fresh Foods Co.",
            ImportDate = DateTime.Today,
            Status = "Draft",
            Items = new[]
            {
                new { ProductId = 1, Quantity = 50, UnitPrice = 40000, ExpiryDate = DateTime.Today.AddDays(10) }
            }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/imports", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<ImportDetailDto>();

        // Act
        var response = await _client.GetAsync($"/api/imports/{created!.ImportId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var import = await response.Content.ReadFromJsonAsync<ImportDetailDto>();
        import.Should().NotBeNull();
        import!.ImportId.Should().Be(created.ImportId);
        import.Items.Should().HaveCount(1);
        import.Supplier.Should().Be("Fresh Foods Co.");
    }

    [Fact]
    public async Task GetImport_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var importId = 99999;

        // Act
        var response = await _client.GetAsync($"/api/imports/{importId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateImport_ValidData_ReturnsCreated()
    {
        // Arrange
        var createDto = new
        {
            Supplier = "Dairy Direct",
            ImportDate = DateTime.Today,
            Status = "Draft",
            Items = new[]
            {
                new { ProductId = 2, Quantity = 100, UnitPrice = 17000, ExpiryDate = DateTime.Today.AddDays(7) },
                new { ProductId = 3, Quantity = 60, UnitPrice = 50000, ExpiryDate = DateTime.Today.AddDays(3) }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/imports", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<ImportDetailDto>();
        created.Should().NotBeNull();
        created!.Supplier.Should().Contain("Dairy");
        created.Items.Should().HaveCount(2);
        created.TotalCost.Should().BeGreaterThan(0);

        // Verify Location header
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateImport_WithExpiryTracking_CalculatesFlags()
    {
        // Arrange - Create import with items expiring soon
        var createDto = new
        {
            Supplier = "Test Supplier",
            ImportDate = DateTime.Today,
            Status = "Draft",
            Items = new[]
            {
                new { ProductId = 1, Quantity = 30, UnitPrice = 40000, ExpiryDate = DateTime.Today.AddDays(3) } // Near expiry
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/imports", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<ImportDetailDto>();
        created.Should().NotBeNull();
        created!.NearExpiry.Should().BeTrue();
        created.Expired.Should().BeFalse();
    }

    [Fact]
    public async Task CreateImport_WithExpiredItems_SetsExpiredFlag()
    {
        // Arrange - Create import with already expired items
        var createDto = new
        {
            Supplier = "Test Supplier",
            ImportDate = DateTime.Today,
            Status = "Draft",
            Items = new[]
            {
                new { ProductId = 1, Quantity = 20, UnitPrice = 40000, ExpiryDate = DateTime.Today.AddDays(-5) } // Expired
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/imports", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<ImportDetailDto>();
        created.Should().NotBeNull();
        created!.Expired.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateImport_ValidData_ReturnsSuccess()
    {
        // Arrange - Create an import first
        var createDto = new
        {
            Supplier = "Global Groceries",
            ImportDate = DateTime.Today,
            Status = "Draft",
            Items = new[]
            {
                new { ProductId = 4, Quantity = 40, UnitPrice = 90000 }
            }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/imports", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<ImportDetailDto>();

        // Update the import
        var updateDto = new
        {
            ImportId = created!.ImportId,
            Supplier = "Global Groceries",
            ImportDate = DateTime.Today,
            Status = "Completed",
            Items = new[]
            {
                new { ProductId = 4, Quantity = 50, UnitPrice = 90000 } // Updated quantity
            }
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/imports/{created.ImportId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<ImportDetailDto>();
        updated.Should().NotBeNull();
        updated!.Status.Should().Be("Completed");
        updated.Items[0].Quantity.Should().Be(50);
    }

    [Fact]
    public async Task UpdateImport_MismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var updateDto = new
        {
            ImportId = 1,
            Supplier = "Test",
            ImportDate = DateTime.Today,
            Status = "Draft",
            Items = new[] { new { ProductId = 1, Quantity = 10, UnitPrice = 10000 } }
        };

        // Act - Using different ID in URL
        var response = await _client.PutAsJsonAsync($"/api/imports/999", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Mismatched import id");
    }

    [Fact]
    public async Task UpdateImport_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new
        {
            ImportId = 99999,
            Supplier = "Test",
            ImportDate = DateTime.Today,
            Status = "Draft",
            Items = new[] { new { ProductId = 1, Quantity = 10, UnitPrice = 10000 } }
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/imports/99999", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteImport_ExistingId_ReturnsNoContent()
    {
        // Arrange - Create an import to delete
        var createDto = new
        {
            Supplier = "Test Supplier",
            ImportDate = DateTime.Today,
            Status = "Draft",
            Items = new[]
            {
                new { ProductId = 1, Quantity = 25, UnitPrice = 40000 }
            }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/imports", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<ImportDetailDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/imports/{created!.ImportId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/imports/{created.ImportId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteImport_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var importId = 99999;

        // Act
        var response = await _client.DeleteAsync($"/api/imports/{importId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateImport_MultipleItems_CalculatesTotalCorrectly()
    {
        // Arrange
        var createDto = new
        {
            Supplier = "Premium Produce",
            ImportDate = DateTime.Today,
            Status = "Draft",
            Items = new[]
            {
                new { ProductId = 1, Quantity = 50, UnitPrice = 40000 },  // 2,000,000
                new { ProductId = 2, Quantity = 100, UnitPrice = 17000 }, // 1,700,000
                new { ProductId = 3, Quantity = 30, UnitPrice = 50000 }   // 1,500,000
                // Expected total: 5,200,000
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/imports", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<ImportDetailDto>();
        created.Should().NotBeNull();
        created!.TotalCost.Should().Be(5200000);
        created.ItemCount.Should().Be(3);
    }

    [Fact]
    public async Task ImportWorkflow_CreateUpdateDelete_Success()
    {
        // Step 1: Create import
        var createDto = new
        {
            Supplier = "Workflow Test Supplier",
            ImportDate = DateTime.Today,
            Status = "Draft",
            Items = new[]
            {
                new { ProductId = 1, Quantity = 40, UnitPrice = 45000 }
            }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/imports", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ImportDetailDto>();

        // Step 2: Get the import
        var getResponse = await _client.GetAsync($"/api/imports/{created!.ImportId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 3: Update the import
        var updateDto = new
        {
            ImportId = created.ImportId,
            Supplier = "Workflow Test Supplier",
            ImportDate = DateTime.Today,
            Status = "Completed",
            Items = new[]
            {
                new { ProductId = 1, Quantity = 45, UnitPrice = 45000 }
            }
        };
        var updateResponse = await _client.PutAsJsonAsync($"/api/imports/{created.ImportId}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 4: Delete the import
        var deleteResponse = await _client.DeleteAsync($"/api/imports/{created.ImportId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
