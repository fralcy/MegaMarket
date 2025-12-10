using Microsoft.EntityFrameworkCore;
using MegaMarket.API.DTOs.Dashboard.Employee;
using MegaMarket.API.DTOs.Dashboard.Common;
using MegaMarket.API.Helpers;
using MegaMarket.Data.Data;

namespace MegaMarket.API.Services;

public class DashboardEmployeeService
{
    private readonly MegaMarketDbContext _context;

    public DashboardEmployeeService(MegaMarketDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeDashboardDto> GetEmployeeDashboardAsync(DateRangeEnum dateRange, string? role, int userId)
    {
        var period = DateRangeHelper.GetDateRange(dateRange);

        var attendanceQuery = _context.Attendances
            .AsNoTracking()
            .Where(a => a.CheckIn >= period.StartDate && a.CheckIn <= period.EndDate);

        // Employee chỉ xem được dữ liệu của chính mình
        if (role != "Admin")
        {
            attendanceQuery = attendanceQuery.Where(a => a.UserId == userId);
        }

        var totalAttendances = await attendanceQuery.CountAsync();

        var onTimeCount = await attendanceQuery
            .Where(a => !a.IsLate)
            .CountAsync();

        var lateCount = await attendanceQuery
            .Where(a => a.IsLate)
            .CountAsync();

        // Tính tổng số giờ làm việc
        var totalHoursWorked = await attendanceQuery
            .Where(a => a.CheckOut != null)
            .Select(a => EF.Functions.DateDiffMinute(a.CheckIn, a.CheckOut.Value) / 60.0)
            .SumAsync();

        // Attendance by shift
        var attendanceByShift = await attendanceQuery
            .Include(a => a.ShiftType)
            .GroupBy(a => new { a.ShiftType.ShiftName, a.ShiftType.WagePerHour })
            .Select(g => new AttendanceByShiftDto
            {
                ShiftName = g.Key.ShiftName,
                AttendanceCount = g.Count(),
                OnTimeCount = g.Count(a => !a.IsLate),
                LateCount = g.Count(a => a.IsLate),
                TotalHoursWorked = g.Where(a => a.CheckOut != null)
                    .Sum(a => EF.Functions.DateDiffMinute(a.CheckIn, a.CheckOut.Value) / 60.0),
                TotalWages = g.Where(a => a.CheckOut != null)
                    .Sum(a => EF.Functions.DateDiffMinute(a.CheckIn, a.CheckOut.Value) / 60.0 * g.Key.WagePerHour)
            })
            .ToListAsync();

        // Chỉ Admin mới xem được thông tin tổng số nhân viên và tổng lương
        int? totalEmployees = null;
        decimal? totalWages = null;

        if (role == "Admin")
        {
            totalEmployees = await _context.Users
                .Where(u => u.Role == "Employee")
                .CountAsync();

            totalWages = attendanceByShift.Sum(s => s.TotalWages);
        }

        return new EmployeeDashboardDto
        {
            TotalEmployees = totalEmployees,
            TotalAttendances = totalAttendances,
            OnTimePercentage = totalAttendances > 0 ? (double)onTimeCount / totalAttendances * 100 : 0,
            LatePercentage = totalAttendances > 0 ? (double)lateCount / totalAttendances * 100 : 0,
            TotalHoursWorked = totalHoursWorked,
            TotalWages = totalWages,
            AttendanceByShift = attendanceByShift
        };
    }

    public async Task<List<EmployeeWorkSummaryDto>> GetEmployeeWorkSummaryAsync(DateRangeEnum dateRange)
    {
        var period = DateRangeHelper.GetDateRange(dateRange);

        var employeeSummaries = await _context.Attendances
            .AsNoTracking()
            .Where(a => a.CheckIn >= period.StartDate && a.CheckIn <= period.EndDate)
            .Include(a => a.User)
            .Include(a => a.ShiftType)
            .GroupBy(a => new
            {
                UserId = a.UserId,
                Username = a.User.Username,
                FullName = a.User.FullName
            })
            .Select(g => new EmployeeWorkSummaryDto
            {
                UserId = g.Key.UserId,
                Username = g.Key.Username,
                FullName = g.Key.FullName,
                DaysWorked = g.Select(a => a.CheckIn.Date).Distinct().Count(),
                TotalHoursWorked = g.Where(a => a.CheckOut != null)
                    .Sum(a => EF.Functions.DateDiffMinute(a.CheckIn, a.CheckOut.Value) / 60.0),
                LateCount = g.Count(a => a.IsLate),
                TotalWages = g.Where(a => a.CheckOut != null)
                    .Sum(a => EF.Functions.DateDiffMinute(a.CheckIn, a.CheckOut.Value) / 60.0 * a.ShiftType.WagePerHour),
                AttendanceRate = g.Count() > 0 ? (double)g.Count(a => !a.IsLate) / g.Count() * 100 : 0
            })
            .OrderByDescending(e => e.TotalHoursWorked)
            .ToListAsync();

        return employeeSummaries;
    }

    public async Task<AttendanceRateDto> GetAttendanceStatisticsAsync(int employeeId, DateRangeEnum dateRange)
    {
        var period = DateRangeHelper.GetDateRange(dateRange);

        var attendances = await _context.Attendances
            .AsNoTracking()
            .Where(a => a.UserId == employeeId &&
                       a.CheckIn >= period.StartDate &&
                       a.CheckIn <= period.EndDate)
            .Include(a => a.ShiftType)
            .ToListAsync();

        var totalAttendances = attendances.Count;
        var onTimeCount = attendances.Count(a => !a.IsLate);
        var lateCount = attendances.Count(a => a.IsLate);

        var totalHoursWorked = attendances
            .Where(a => a.CheckOut != null)
            .Sum(a => (a.CheckOut.Value - a.CheckIn).TotalHours);

        var totalWages = attendances
            .Where(a => a.CheckOut != null)
            .Sum(a => (a.CheckOut.Value - a.CheckIn).TotalHours * (double)a.ShiftType.WagePerHour);

        return new AttendanceRateDto
        {
            TotalAttendances = totalAttendances,
            OnTimeCount = onTimeCount,
            LateCount = lateCount,
            OnTimePercentage = totalAttendances > 0 ? (double)onTimeCount / totalAttendances * 100 : 0,
            LatePercentage = totalAttendances > 0 ? (double)lateCount / totalAttendances * 100 : 0,
            TotalHoursWorked = totalHoursWorked,
            TotalWages = totalWages
        };
    }
}
