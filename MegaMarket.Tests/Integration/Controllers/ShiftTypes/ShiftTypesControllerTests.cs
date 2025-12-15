using MegaMarket.Data.Models;
using MegaMarket.API.DTOs;

namespace MegaMarket.Tests.Integration.Controllers.ShiftTypes;

public class ShiftTypesControllerTests : IClassFixture<TestServerFixture>
{
    private readonly HttpClient _client;
    private readonly TestServerFixture _factory;

    public ShiftTypesControllerTests(TestServerFixture factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllShiftTypes_ReturnsSuccess_WithListOfShiftTypes()
    {
        // Act
        var response = await _client.GetAsync("/api/ShiftTypes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var shiftTypes = await response.Content.ReadFromJsonAsync<List<ShiftType>>();
        shiftTypes.Should().NotBeNull();
        shiftTypes.Should().HaveCountGreaterThan(0);
        shiftTypes.Should().Contain(s => s.Name == "Morning Shift");
    }

    [Fact]
    public async Task GetShiftType_ExistingId_ReturnsShiftType()
    {
        // Arrange
        var shiftTypeId = 1;

        // Act
        var response = await _client.GetAsync($"/api/ShiftTypes/{shiftTypeId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var shiftType = await response.Content.ReadFromJsonAsync<ShiftType>();
        shiftType.Should().NotBeNull();
        shiftType!.ShiftTypeId.Should().Be(shiftTypeId);
        shiftType.Name.Should().Be("Morning Shift");
    }

    [Fact]
    public async Task GetShiftType_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var shiftTypeId = 99999;

        // Act
        var response = await _client.GetAsync($"/api/ShiftTypes/{shiftTypeId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetShiftTypeByName_ExistingName_ReturnsShiftType()
    {
        // Arrange
        var shiftName = "Morning Shift";

        // Act
        var response = await _client.GetAsync($"/api/ShiftTypes/name/{shiftName}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var shiftType = await response.Content.ReadFromJsonAsync<ShiftType>();
        shiftType.Should().NotBeNull();
        shiftType!.Name.Should().Be(shiftName);
    }

    [Fact]
    public async Task GetShiftTypeByName_NonExistingName_ReturnsNotFound()
    {
        // Arrange
        var shiftName = "NonExistentShift";

        // Act
        var response = await _client.GetAsync($"/api/ShiftTypes/name/{shiftName}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateShiftType_ValidData_ReturnsCreated()
    {
        // Arrange
        var newShiftType = new ShiftTypeInputDto
        {
            Name = "Evening Shift",
            StartTime = new TimeSpan(18, 0, 0),  // 6:00 PM
            EndTime = new TimeSpan(23, 0, 0),     // 11:00 PM
            WagePerHour = 50000
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/ShiftTypes", newShiftType);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdShiftType = await response.Content.ReadFromJsonAsync<ShiftType>();
        createdShiftType.Should().NotBeNull();
        createdShiftType!.Name.Should().Be(newShiftType.Name);
        createdShiftType.StartTime.Should().Be(newShiftType.StartTime);
        createdShiftType.EndTime.Should().Be(newShiftType.EndTime);
        createdShiftType.WagePerHour.Should().Be(newShiftType.WagePerHour);

        // Verify Location header contains shift type ID
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain(createdShiftType.ShiftTypeId.ToString());
    }

    [Fact]
    public async Task CreateShiftType_DuplicateName_ReturnsBadRequest()
    {
        // Arrange
        var duplicateShiftType = new ShiftTypeInputDto
        {
            Name = "Morning Shift", // Name already exists
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(17, 0, 0),
            WagePerHour = 40000
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/ShiftTypes", duplicateShiftType);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateShiftType_ValidData_ReturnsSuccess()
    {
        // Arrange - First create a shift type to update
        var newShiftType = new ShiftTypeInputDto
        {
            Name = "Shift To Update",
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(18, 0, 0),
            WagePerHour = 45000
        };
        var createResponse = await _client.PostAsJsonAsync("/api/ShiftTypes", newShiftType);
        var createdShiftType = await createResponse.Content.ReadFromJsonAsync<ShiftType>();

        var updateDto = new ShiftTypeInputDto
        {
            Name = "Updated Shift Name",
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(19, 0, 0),
            WagePerHour = 55000
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/ShiftTypes/{createdShiftType!.ShiftTypeId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedShiftType = await response.Content.ReadFromJsonAsync<ShiftType>();
        updatedShiftType.Should().NotBeNull();
        updatedShiftType!.Name.Should().Be(updateDto.Name);
        updatedShiftType.StartTime.Should().Be(updateDto.StartTime);
        updatedShiftType.EndTime.Should().Be(updateDto.EndTime);
        updatedShiftType.WagePerHour.Should().Be(updateDto.WagePerHour);
    }

    [Fact]
    public async Task UpdateShiftType_NonExistingId_ReturnsBadRequest()
    {
        // Arrange
        var shiftTypeId = 99999;
        var updateDto = new ShiftTypeInputDto
        {
            Name = "Non Existing Shift",
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(17, 0, 0),
            WagePerHour = 40000
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/ShiftTypes/{shiftTypeId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteShiftType_ExistingId_ReturnsSuccess()
    {
        // Arrange - Create a shift type to delete
        var newShiftType = new ShiftTypeInputDto
        {
            Name = "Shift To Delete",
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(17, 0, 0),
            WagePerHour = 40000
        };
        var createResponse = await _client.PostAsJsonAsync("/api/ShiftTypes", newShiftType);
        var createdShiftType = await createResponse.Content.ReadFromJsonAsync<ShiftType>();

        // Act
        var response = await _client.DeleteAsync($"/api/ShiftTypes/{createdShiftType!.ShiftTypeId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var message = await response.Content.ReadAsStringAsync();
        message.Should().Contain("deleted successfully");

        // Verify shift type is deleted
        var getResponse = await _client.GetAsync($"/api/ShiftTypes/{createdShiftType.ShiftTypeId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteShiftType_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var shiftTypeId = 99999;

        // Act
        var response = await _client.DeleteAsync($"/api/ShiftTypes/{shiftTypeId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
