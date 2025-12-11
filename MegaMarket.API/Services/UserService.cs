using Microsoft.EntityFrameworkCore;
using MegaMarket.Data.Data;
using MegaMarket.Data.Models;
using MegaMarket.API.DTOs;

namespace MegaMarket.API.Services;

public class UserService
{
    private readonly MegaMarketDbContext _context;

    public UserService(MegaMarketDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> CreateUserAsync(UserInputDto input)
    {
        // Kiểm tra username đã tồn tại chưa
        var existingUser = await GetUserByUsernameAsync(input.Username);
        if (existingUser != null)
        {
            throw new Exception("Username đã tồn tại!");
        }

        // Hash password trước khi lưu
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(input.Password);

        var user = new User
        {
            FullName = input.FullName,
            Username = input.Username,
            Password = hashedPassword, // Lưu password đã hash
            Role = input.Role,
            Phone = input.Phone,
            Email = input.Email
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> UpdateUserAsync(int userId, UserInputDto input)
    {
        var user = await GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("Không tìm thấy nhân viên!");
        }

        // Kiểm tra username mới có bị trùng không
        if (input.Username != user.Username)
        {
            var existingUser = await GetUserByUsernameAsync(input.Username);
            if (existingUser != null)
            {
                throw new Exception("Username đã tồn tại!");
            }
        }

        user.FullName = input.FullName;
        user.Username = input.Username;

        // Chỉ update password nếu có password mới
        if (!string.IsNullOrEmpty(input.Password))
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
        }

        user.Role = input.Role;
        user.Phone = input.Phone;
        user.Email = input.Email;

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await GetUserByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }
}