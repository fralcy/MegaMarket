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
    public async Task GetImports_ReturnsSuccess()
    {
        // Act
        var response = await _client.GetAsync("/api/imports");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
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
    public async Task DeleteImport_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var importId = 99999;

        // Act
        var response = await _client.DeleteAsync($"/api/imports/{importId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
