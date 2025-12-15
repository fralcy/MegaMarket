using MegaMarket.Data.Models;
using MegaMarket.API.DTOs;

namespace MegaMarket.Tests.Integration.Controllers.Attendance;

public class AttendanceControllerTests : IClassFixture<TestServerFixture>
{
    private readonly HttpClient _client;
    private readonly TestServerFixture _factory;

    public AttendanceControllerTests(TestServerFixture factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllAttendances_ReturnsSuccess_WithListOfAttendances()
    {
        // Act
        var response = await _client.GetAsync("/api/Attendance");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var attendances = await response.Content.ReadFromJsonAsync<List<MegaMarket.Data.Models.Attendance>>();
        attendances.Should().NotBeNull();
        attendances.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetAttendance_ExistingId_ReturnsAttendance()
    {
        // Arrange
        var attendanceId = 1;

        // Act
        var response = await _client.GetAsync($"/api/Attendance/{attendanceId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var attendance = await response.Content.ReadFromJsonAsync<MegaMarket.Data.Models.Attendance>();
        attendance.Should().NotBeNull();
        attendance!.AttendanceId.Should().Be(attendanceId);
    }

    [Fact]
    public async Task GetAttendance_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var attendanceId = 99999;

        // Act
        var response = await _client.GetAsync($"/api/Attendance/{attendanceId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAttendancesByUser_ExistingUserId_ReturnsAttendances()
    {
        // Arrange
        var userId = 1;

        // Act
        var response = await _client.GetAsync($"/api/Attendance/user/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var attendances = await response.Content.ReadFromJsonAsync<List<MegaMarket.Data.Models.Attendance>>();
        attendances.Should().NotBeNull();
        attendances!.All(a => a.UserId == userId).Should().BeTrue();
    }

    [Fact]
    public async Task GetAttendancesByDate_ExistingDate_ReturnsAttendances()
    {
        // Arrange
        var date = DateTime.Today;

        // Act
        var response = await _client.GetAsync($"/api/Attendance/date/{date:yyyy-MM-dd}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var attendances = await response.Content.ReadFromJsonAsync<List<MegaMarket.Data.Models.Attendance>>();
        attendances.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAttendance_ValidData_ReturnsCreated()
    {
        // Arrange
        var newAttendance = new AttendanceInputDto
        {
            UserId = 1,
            ShiftTypeId = 1,
            Date = DateTime.Today.AddDays(1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Attendance", newAttendance);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdAttendance = await response.Content.ReadFromJsonAsync<MegaMarket.Data.Models.Attendance>();
        createdAttendance.Should().NotBeNull();
        createdAttendance!.UserId.Should().Be(newAttendance.UserId);
        createdAttendance.ShiftTypeId.Should().Be(newAttendance.ShiftTypeId);
        createdAttendance.Date.Date.Should().Be(newAttendance.Date.Date);

        // Verify Location header contains attendance ID
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain(createdAttendance.AttendanceId.ToString());
    }

    [Fact]
    public async Task CreateAttendance_DuplicateUserAndDate_ReturnsBadRequest()
    {
        // Arrange - First create an attendance record
        var newAttendance = new AttendanceInputDto
        {
            UserId = 1,
            ShiftTypeId = 1,
            Date = DateTime.Today.AddDays(2)
        };
        await _client.PostAsJsonAsync("/api/Attendance", newAttendance);

        // Try to create another attendance for the same user and date
        var duplicateAttendance = new AttendanceInputDto
        {
            UserId = 1,
            ShiftTypeId = 2,
            Date = DateTime.Today.AddDays(2)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Attendance", duplicateAttendance);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CheckIn_ValidData_ReturnsSuccess()
    {
        // Arrange - Create an attendance record first
        var attendance = new AttendanceInputDto
        {
            UserId = 1,
            ShiftTypeId = 1,
            Date = DateTime.Today.AddDays(3)
        };
        await _client.PostAsJsonAsync("/api/Attendance", attendance);

        var checkInDto = new CheckInOutDto
        {
            UserId = 1,
            Date = DateTime.Today.AddDays(3)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Attendance/check-in", checkInDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var checkedInAttendance = await response.Content.ReadFromJsonAsync<MegaMarket.Data.Models.Attendance>();
        checkedInAttendance.Should().NotBeNull();
        checkedInAttendance!.CheckIn.Should().NotBeNull();
    }

    [Fact]
    public async Task CheckOut_AfterCheckIn_ReturnsSuccess()
    {
        // Arrange - Create attendance and check in
        var attendance = new AttendanceInputDto
        {
            UserId = 1,
            ShiftTypeId = 1,
            Date = DateTime.Today.AddDays(4)
        };
        await _client.PostAsJsonAsync("/api/Attendance", attendance);

        var checkInDto = new CheckInOutDto
        {
            UserId = 1,
            Date = DateTime.Today.AddDays(4)
        };
        await _client.PostAsJsonAsync("/api/Attendance/check-in", checkInDto);

        var checkOutDto = new CheckInOutDto
        {
            UserId = 1,
            Date = DateTime.Today.AddDays(4)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Attendance/check-out", checkOutDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var checkedOutAttendance = await response.Content.ReadFromJsonAsync<MegaMarket.Data.Models.Attendance>();
        checkedOutAttendance.Should().NotBeNull();
        checkedOutAttendance!.CheckIn.Should().NotBeNull();
        checkedOutAttendance.CheckOut.Should().NotBeNull();
        checkedOutAttendance.CheckOut.Should().BeAfter(checkedOutAttendance.CheckIn!.Value);
    }

    [Fact]
    public async Task CheckOut_WithoutCheckIn_ReturnsBadRequest()
    {
        // Arrange - Create attendance without checking in
        var attendance = new AttendanceInputDto
        {
            UserId = 1,
            ShiftTypeId = 1,
            Date = DateTime.Today.AddDays(5)
        };
        await _client.PostAsJsonAsync("/api/Attendance", attendance);

        var checkOutDto = new CheckInOutDto
        {
            UserId = 1,
            Date = DateTime.Today.AddDays(5)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Attendance/check-out", checkOutDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAttendance_ValidData_ReturnsSuccess()
    {
        // Arrange - Create an attendance to update
        var newAttendance = new AttendanceInputDto
        {
            UserId = 1,
            ShiftTypeId = 1,
            Date = DateTime.Today.AddDays(6)
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Attendance", newAttendance);
        var createdAttendance = await createResponse.Content.ReadFromJsonAsync<MegaMarket.Data.Models.Attendance>();

        var updateDto = new AttendanceInputDto
        {
            UserId = 1,
            ShiftTypeId = 2,
            Date = DateTime.Today.AddDays(6),
            Note = "Updated note"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Attendance/{createdAttendance!.AttendanceId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedAttendance = await response.Content.ReadFromJsonAsync<MegaMarket.Data.Models.Attendance>();
        updatedAttendance.Should().NotBeNull();
        updatedAttendance!.ShiftTypeId.Should().Be(updateDto.ShiftTypeId);
        updatedAttendance.Note.Should().Be(updateDto.Note);
    }

    [Fact]
    public async Task UpdateAttendance_NonExistingId_ReturnsBadRequest()
    {
        // Arrange
        var attendanceId = 99999;
        var updateDto = new AttendanceInputDto
        {
            UserId = 1,
            ShiftTypeId = 1,
            Date = DateTime.Today
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Attendance/{attendanceId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteAttendance_ExistingId_ReturnsSuccess()
    {
        // Arrange - Create an attendance to delete
        var newAttendance = new AttendanceInputDto
        {
            UserId = 1,
            ShiftTypeId = 1,
            Date = DateTime.Today.AddDays(7)
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Attendance", newAttendance);
        var createdAttendance = await createResponse.Content.ReadFromJsonAsync<MegaMarket.Data.Models.Attendance>();

        // Act
        var response = await _client.DeleteAsync($"/api/Attendance/{createdAttendance!.AttendanceId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var message = await response.Content.ReadAsStringAsync();
        message.Should().Contain("deleted successfully");

        // Verify attendance is deleted
        var getResponse = await _client.GetAsync($"/api/Attendance/{createdAttendance.AttendanceId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAttendance_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var attendanceId = 99999;

        // Act
        var response = await _client.DeleteAsync($"/api/Attendance/{attendanceId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CheckIn_TwiceOnSameDay_ReturnsBadRequest()
    {
        // Arrange - Create attendance and check in once
        var attendance = new AttendanceInputDto
        {
            UserId = 1,
            ShiftTypeId = 1,
            Date = DateTime.Today.AddDays(8)
        };
        await _client.PostAsJsonAsync("/api/Attendance", attendance);

        var checkInDto = new CheckInOutDto
        {
            UserId = 1,
            Date = DateTime.Today.AddDays(8)
        };
        await _client.PostAsJsonAsync("/api/Attendance/check-in", checkInDto);

        // Act - Try to check in again
        var response = await _client.PostAsJsonAsync("/api/Attendance/check-in", checkInDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
