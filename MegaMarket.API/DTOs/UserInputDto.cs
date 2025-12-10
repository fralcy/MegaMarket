namespace MegaMarket.API.DTOs;

public class UserInputDto
{
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Employee";
    public string? Phone { get; set; }
    public string? Email { get; set; }
}