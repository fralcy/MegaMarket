using Microsoft.EntityFrameworkCore;
using MegaMarket.Data.Data;
using MegaMarket.Data.Models;
using MegaMarket.API.DTOs;

namespace MegaMarket.API.Services;

public class AttendanceService
{
    private readonly MegaMarketDbContext _context;

    public AttendanceService(MegaMarketDbContext context)
    {
        _context = context;
    }

    // Lấy tất cả bản ghi chấm công
    public async Task<List<Attendance>> GetAllAttendancesAsync()
    {
        return await _context.Attendances
            .Include(a => a.User)
            .Include(a => a.ShiftType)
            .ToListAsync();
    }

    // Lấy chấm công theo ID
    public async Task<Attendance?> GetAttendanceByIdAsync(int attendanceId)
    {
        return await _context.Attendances
            .Include(a => a.User)
            .Include(a => a.ShiftType)
            .FirstOrDefaultAsync(a => a.AttendanceId == attendanceId);
    }

    // Lấy chấm công theo nhân viên
    public async Task<List<Attendance>> GetAttendancesByUserIdAsync(int userId)
    {
        return await _context.Attendances
            .Include(a => a.User)
            .Include(a => a.ShiftType)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }

    // Lấy chấm công theo ngày
    public async Task<List<Attendance>> GetAttendancesByDateAsync(DateTime date)
    {
        return await _context.Attendances
            .Include(a => a.User)
            .Include(a => a.ShiftType)
            .Where(a => a.Date.Date == date.Date)
            .ToListAsync();
    }

    // Tạo bản ghi chấm công
    public async Task<Attendance> CreateAttendanceAsync(AttendanceInputDto input)
    {
        // Kiểm tra đã có bản ghi chấm công trong ngày chưa
        var existing = await _context.Attendances
            .FirstOrDefaultAsync(a => a.UserId == input.UserId && a.Date.Date == input.Date.Date);

        if (existing != null)
        {
            throw new Exception("Đã có bản ghi chấm công trong ngày này!");
        }

        // Lấy thông tin ca làm việc để kiểm tra đi trễ
        var shiftType = await _context.ShiftTypes.FindAsync(input.ShiftTypeId);
        if (shiftType == null)
        {
            throw new Exception("Không tìm thấy loại ca làm việc!");
        }

        bool isLate = false;
        if (input.CheckIn.HasValue)
        {
            var checkInTime = input.CheckIn.Value.TimeOfDay;
            isLate = checkInTime > shiftType.StartTime;
        }

        var attendance = new Attendance
        {
            UserId = input.UserId,
            ShiftTypeId = input.ShiftTypeId,
            Date = input.Date.Date,
            CheckIn = input.CheckIn,
            CheckOut = input.CheckOut,
            IsLate = isLate,
            Note = input.Note
        };

        _context.Attendances.Add(attendance);
        await _context.SaveChangesAsync();

        return await GetAttendanceByIdAsync(attendance.AttendanceId) ?? attendance;
    }

    // Check-in
    public async Task<Attendance> CheckInAsync(int userId, DateTime date)
    {
        var attendance = await _context.Attendances
            .Include(a => a.ShiftType)
            .FirstOrDefaultAsync(a => a.UserId == userId && a.Date.Date == date.Date);

        if (attendance == null)
        {
            throw new Exception("Không tìm thấy bản ghi chấm công!");
        }

        if (attendance.CheckIn.HasValue)
        {
            throw new Exception("Đã check-in rồi!");
        }

        var checkInTime = DateTime.Now;
        attendance.CheckIn = checkInTime;
        attendance.IsLate = checkInTime.TimeOfDay > attendance.ShiftType.StartTime;

        await _context.SaveChangesAsync();
        return attendance;
    }

    // Check-out
    public async Task<Attendance> CheckOutAsync(int userId, DateTime date)
    {
        var attendance = await _context.Attendances
            .FirstOrDefaultAsync(a => a.UserId == userId && a.Date.Date == date.Date);

        if (attendance == null)
        {
            throw new Exception("Không tìm thấy bản ghi chấm công!");
        }

        if (!attendance.CheckIn.HasValue)
        {
            throw new Exception("Chưa check-in!");
        }

        if (attendance.CheckOut.HasValue)
        {
            throw new Exception("Đã check-out rồi!");
        }

        attendance.CheckOut = DateTime.Now;
        await _context.SaveChangesAsync();
        return attendance;
    }

    // Cập nhật bản ghi chấm công
    public async Task<Attendance> UpdateAttendanceAsync(int attendanceId, AttendanceInputDto input)
    {
        var attendance = await GetAttendanceByIdAsync(attendanceId);
        if (attendance == null)
        {
            throw new Exception("Không tìm thấy bản ghi chấm công!");
        }

        var shiftType = await _context.ShiftTypes.FindAsync(input.ShiftTypeId);
        if (shiftType == null)
        {
            throw new Exception("Không tìm thấy loại ca làm việc!");
        }

        bool isLate = false;
        if (input.CheckIn.HasValue)
        {
            var checkInTime = input.CheckIn.Value.TimeOfDay;
            isLate = checkInTime > shiftType.StartTime;
        }

        attendance.ShiftTypeId = input.ShiftTypeId;
        attendance.Date = input.Date.Date;
        attendance.CheckIn = input.CheckIn;
        attendance.CheckOut = input.CheckOut;
        attendance.IsLate = isLate;
        attendance.Note = input.Note;

        await _context.SaveChangesAsync();
        return attendance;
    }

    // Xóa bản ghi chấm công
    public async Task<bool> DeleteAttendanceAsync(int attendanceId)
    {
        var attendance = await GetAttendanceByIdAsync(attendanceId);
        if (attendance == null)
        {
            return false;
        }

        _context.Attendances.Remove(attendance);
        await _context.SaveChangesAsync();
        return true;
    }
}