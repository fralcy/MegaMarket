namespace MegaMarket.API.DTOs.User
{
    public class UserResDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = "Employee"; // Admin / Employee
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
