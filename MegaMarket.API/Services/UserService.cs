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

    // Lấy tất cả nhân viên
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    // Lấy nhân viên theo ID
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    // Lấy nhân viên theo username
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    // Tạo nhân viên mới
    public async Task<User> CreateUserAsync(UserInputDto input)
    {
        // Kiểm tra username đã tồn tại chưa
        var existingUser = await GetUserByUsernameAsync(input.Username);
        if (existingUser != null)
        {
            throw new Exception("Username đã tồn tại!");
        }

        var user = new User
        {
            FullName = input.FullName,
            Username = input.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(input.Password), // Hash password
            Role = input.Role,
            Phone = input.Phone,
            Email = input.Email
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // Cập nhật thông tin nhân viên
    public async Task<User> UpdateUserAsync(int userId, UserInputDto input)
    {
        var user = await GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("Không tìm thấy nhân viên!");
        }

        // Nếu đổi username, kiểm tra trùng
        if (user.Username != input.Username)
        {
            var existingUser = await GetUserByUsernameAsync(input.Username);
            if (existingUser != null)
            {
                throw new Exception("Username đã tồn tại!");
            }
            user.Username = input.Username;
        }

        user.FullName = input.FullName;
        user.Role = input.Role;
        user.Phone = input.Phone;
        user.Email = input.Email;

        // Chỉ update password nếu có nhập mới
        if (!string.IsNullOrEmpty(input.Password))
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
        }

        await _context.SaveChangesAsync();
        return user;
    }

    // Xóa nhân viên
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