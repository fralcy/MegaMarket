using MegaMarket.Data.Models;
using MegaMarket.API.DTOs;

namespace MegaMarket.Tests.Integration.Controllers.Users;

public class UsersControllerTests : IClassFixture<TestServerFixture>
{
    private readonly HttpClient _client;
    private readonly TestServerFixture _factory;

    public UsersControllerTests(TestServerFixture factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllUsers_ReturnsSuccess_WithListOfUsers()
    {
        // Act
        var response = await _client.GetAsync("/api/Users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<User>>();
        users.Should().NotBeNull();
        users.Should().HaveCountGreaterThan(0);
        users.Should().Contain(u => u.Username == "admin");
    }

    [Fact]
    public async Task GetUser_ExistingId_ReturnsUser()
    {
        // Arrange
        var userId = 1;

        // Act
        var response = await _client.GetAsync($"/api/Users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<User>();
        user.Should().NotBeNull();
        user!.UserId.Should().Be(userId);
        user.Username.Should().Be("admin");
    }

    [Fact]
    public async Task GetUser_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var userId = 99999;

        // Act
        var response = await _client.GetAsync($"/api/Users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserByUsername_ExistingUsername_ReturnsUser()
    {
        // Arrange
        var username = "admin";

        // Act
        var response = await _client.GetAsync($"/api/Users/username/{username}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<User>();
        user.Should().NotBeNull();
        user!.Username.Should().Be(username);
    }

    [Fact]
    public async Task GetUserByUsername_NonExistingUsername_ReturnsNotFound()
    {
        // Arrange
        var username = "nonexistentuser";

        // Act
        var response = await _client.GetAsync($"/api/Users/username/{username}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateUser_ValidData_ReturnsCreated()
    {
        // Arrange
        var newUser = new UserInputDto
        {
            FullName = "Test Employee",
            Username = "test.employee",
            Password = "TestPassword123!",
            Role = "Employee",
            Phone = "0909123456",
            Email = "test.employee@example.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Users", newUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdUser = await response.Content.ReadFromJsonAsync<User>();
        createdUser.Should().NotBeNull();
        createdUser!.FullName.Should().Be(newUser.FullName);
        createdUser.Username.Should().Be(newUser.Username);
        createdUser.Role.Should().Be(newUser.Role);
        createdUser.Phone.Should().Be(newUser.Phone);
        createdUser.Email.Should().Be(newUser.Email);

        // Verify Location header contains user ID
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain(createdUser.UserId.ToString());
    }

    [Fact]
    public async Task CreateUser_DuplicateUsername_ReturnsBadRequest()
    {
        // Arrange
        var duplicateUser = new UserInputDto
        {
            FullName = "Duplicate User",
            Username = "admin", // Username already exists
            Password = "Password123!",
            Role = "Employee"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Users", duplicateUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUser_ValidData_ReturnsSuccess()
    {
        // Arrange - First create a user to update
        var newUser = new UserInputDto
        {
            FullName = "User To Update",
            Username = "user.update",
            Password = "Password123!",
            Role = "Employee"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Users", newUser);
        var createdUser = await createResponse.Content.ReadFromJsonAsync<User>();

        var updateDto = new UserInputDto
        {
            FullName = "Updated User Name",
            Username = "user.update",
            Password = "NewPassword123!",
            Role = "Employee",
            Phone = "0901234567",
            Email = "updated@example.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Users/{createdUser!.UserId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedUser = await response.Content.ReadFromJsonAsync<User>();
        updatedUser.Should().NotBeNull();
        updatedUser!.FullName.Should().Be(updateDto.FullName);
        updatedUser.Phone.Should().Be(updateDto.Phone);
        updatedUser.Email.Should().Be(updateDto.Email);
    }

    [Fact]
    public async Task UpdateUser_NonExistingId_ReturnsBadRequest()
    {
        // Arrange
        var userId = 99999;
        var updateDto = new UserInputDto
        {
            FullName = "Non Existing User",
            Username = "nonexistent",
            Password = "Password123!",
            Role = "Employee"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Users/{userId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteUser_ExistingId_ReturnsSuccess()
    {
        // Arrange - Create a user to delete
        var newUser = new UserInputDto
        {
            FullName = "User To Delete",
            Username = "user.delete",
            Password = "Password123!",
            Role = "Employee"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Users", newUser);
        var createdUser = await createResponse.Content.ReadFromJsonAsync<User>();

        // Act
        var response = await _client.DeleteAsync($"/api/Users/{createdUser!.UserId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var message = await response.Content.ReadAsStringAsync();
        message.Should().Contain("deleted successfully");

        // Verify user is deleted
        var getResponse = await _client.GetAsync($"/api/Users/{createdUser.UserId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteUser_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var userId = 99999;

        // Act
        var response = await _client.DeleteAsync($"/api/Users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
