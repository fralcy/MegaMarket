using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MegaMarket.Data.Data;
using MegaMarket.API.DTOs;

namespace MegaMarket.API.Services;

public class AuthService
{
    private readonly MegaMarketDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(MegaMarketDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// <summary>
    /// Đăng nhập và trả về JWT token
    /// </summary>
    public async Task<LoginResultDto?> LoginAsync(LoginInputDto input)
    {
        // 1. Tìm user theo username
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == input.Username);

        if (user == null)
        {
            return null; // User không tồn tại
        }

        // 2. Verify password (so sánh với hash trong DB)
        if (!BCrypt.Net.BCrypt.Verify(input.Password, user.Password))
        {
            return null; // Sai password
        }

        // 3. Tạo JWT token
        var token = GenerateJwtToken(user.UserId, user.Username, user.Role);
        var expiresAt = DateTime.UtcNow.AddHours(8); // Token hết hạn sau 8 giờ

        return new LoginResultDto
        {
            Token = token,
            UserId = user.UserId,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            ExpiresAt = expiresAt
        };
    }

    /// <summary>
    /// Tạo JWT token với user info
    /// </summary>
    private string GenerateJwtToken(int userId, string username, string role)
    {
        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new Exception("JWT Key chưa được config!");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "MegaMarket";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "MegaMarket";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Claims chứa thông tin user
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim("userId", userId.ToString()), // Custom claim for Blazor frontend
            new Claim("role", role) // Custom claim for Blazor frontend
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}