using Microsoft.EntityFrameworkCore;
using MegaMarket.Data.Data;
using MegaMarket.Data.Models;
using MegaMarket.API.DTOs;

namespace MegaMarket.API.Services;

public class ShiftTypeService
{
    private readonly MegaMarketDbContext _context;

    public ShiftTypeService(MegaMarketDbContext context)
    {
        _context = context;
    }

    // Lấy tất cả loại ca làm việc
    public async Task<List<ShiftType>> GetAllShiftTypesAsync()
    {
        return await _context.ShiftTypes
            .OrderBy(st => st.StartTime)
            .ToListAsync();
    }

    // Lấy loại ca theo ID
    public async Task<ShiftType?> GetShiftTypeByIdAsync(int shiftTypeId)
    {
        return await _context.ShiftTypes.FindAsync(shiftTypeId);
    }

    // Lấy loại ca theo tên
    public async Task<ShiftType?> GetShiftTypeByNameAsync(string name)
    {
        return await _context.ShiftTypes
            .FirstOrDefaultAsync(st => st.Name == name);
    }

    // Tạo loại ca mới
    public async Task<ShiftType> CreateShiftTypeAsync(ShiftTypeInputDto input)
    {
        // Kiểm tra tên ca đã tồn tại chưa
        var existing = await GetShiftTypeByNameAsync(input.Name);
        if (existing != null)
        {
            throw new Exception("Tên ca làm việc đã tồn tại!");
        }

        // Validate thời gian
        if (input.StartTime >= input.EndTime)
        {
            throw new Exception("Giờ bắt đầu phải nhỏ hơn giờ kết thúc!");
        }

        if (input.WagePerHour <= 0)
        {
            throw new Exception("Lương theo giờ phải lớn hơn 0!");
        }

        var shiftType = new ShiftType
        {
            Name = input.Name,
            StartTime = input.StartTime,
            EndTime = input.EndTime,
            WagePerHour = input.WagePerHour
        };

        _context.ShiftTypes.Add(shiftType);
        await _context.SaveChangesAsync();
        return shiftType;
    }

    // Cập nhật loại ca
    public async Task<ShiftType> UpdateShiftTypeAsync(int shiftTypeId, ShiftTypeInputDto input)
    {
        var shiftType = await GetShiftTypeByIdAsync(shiftTypeId);
        if (shiftType == null)
        {
            throw new Exception("Không tìm thấy loại ca làm việc!");
        }

        // Nếu đổi tên, kiểm tra trùng
        if (shiftType.Name != input.Name)
        {
            var existing = await GetShiftTypeByNameAsync(input.Name);
            if (existing != null)
            {
                throw new Exception("Tên ca làm việc đã tồn tại!");
            }
            shiftType.Name = input.Name;
        }

        // Validate thời gian
        if (input.StartTime >= input.EndTime)
        {
            throw new Exception("Giờ bắt đầu phải nhỏ hơn giờ kết thúc!");
        }

        if (input.WagePerHour <= 0)
        {
            throw new Exception("Lương theo giờ phải lớn hơn 0!");
        }

        shiftType.StartTime = input.StartTime;
        shiftType.EndTime = input.EndTime;
        shiftType.WagePerHour = input.WagePerHour;

        await _context.SaveChangesAsync();
        return shiftType;
    }

    // Xóa loại ca
    public async Task<bool> DeleteShiftTypeAsync(int shiftTypeId)
    {
        var shiftType = await GetShiftTypeByIdAsync(shiftTypeId);
        if (shiftType == null)
        {
            return false;
        }

        // Kiểm tra có attendance nào đang dùng không
        var hasAttendances = await _context.Attendances
            .AnyAsync(a => a.ShiftTypeId == shiftTypeId);

        if (hasAttendances)
        {
            throw new Exception("Không thể xóa! Có nhân viên đang sử dụng ca này.");
        }

        _context.ShiftTypes.Remove(shiftType);
        await _context.SaveChangesAsync();
        return true;
    }

    // Tính lương dựa trên giờ làm việc
    public decimal CalculateWage(ShiftType shiftType, DateTime? checkIn, DateTime? checkOut)
    {
        if (!checkIn.HasValue || !checkOut.HasValue)
        {
            return 0;
        }

        var workDuration = checkOut.Value - checkIn.Value;
        var hoursWorked = (decimal)workDuration.TotalHours;

        return hoursWorked * shiftType.WagePerHour;
    }
}