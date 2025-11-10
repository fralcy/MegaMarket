using MegaMarket.Data.Models;
using MegaMarket.API.Services;

namespace MegaMarket.API.GraphQL;

public class Query
{
    // ==================== USER QUERIES ====================

    [GraphQLDescription("Lấy tất cả nhân viên")]
    public async Task<List<User>> GetUsers([Service] UserService userService)
    {
        return await userService.GetAllUsersAsync();
    }

    [GraphQLDescription("Lấy nhân viên theo ID")]
    public async Task<User?> GetUser(
        int userId,
        [Service] UserService userService)
    {
        return await userService.GetUserByIdAsync(userId);
    }

    [GraphQLDescription("Lấy nhân viên theo username")]
    public async Task<User?> GetUserByUsername(
        string username,
        [Service] UserService userService)
    {
        return await userService.GetUserByUsernameAsync(username);
    }

    // ==================== SHIFT TYPE QUERIES ====================

    [GraphQLDescription("Lấy tất cả loại ca làm việc")]
    public async Task<List<ShiftType>> GetShiftTypes([Service] ShiftTypeService shiftTypeService)
    {
        return await shiftTypeService.GetAllShiftTypesAsync();
    }

    [GraphQLDescription("Lấy loại ca theo ID")]
    public async Task<ShiftType?> GetShiftType(
        int shiftTypeId,
        [Service] ShiftTypeService shiftTypeService)
    {
        return await shiftTypeService.GetShiftTypeByIdAsync(shiftTypeId);
    }

    [GraphQLDescription("Lấy loại ca theo tên")]
    public async Task<ShiftType?> GetShiftTypeByName(
        string name,
        [Service] ShiftTypeService shiftTypeService)
    {
        return await shiftTypeService.GetShiftTypeByNameAsync(name);
    }

    // ==================== ATTENDANCE QUERIES ====================

    [GraphQLDescription("Lấy tất cả bản ghi chấm công")]
    public async Task<List<Attendance>> GetAttendances([Service] AttendanceService attendanceService)
    {
        return await attendanceService.GetAllAttendancesAsync();
    }

    [GraphQLDescription("Lấy chấm công theo ID")]
    public async Task<Attendance?> GetAttendance(
        int attendanceId,
        [Service] AttendanceService attendanceService)
    {
        return await attendanceService.GetAttendanceByIdAsync(attendanceId);
    }

    [GraphQLDescription("Lấy lịch sử chấm công của nhân viên")]
    public async Task<List<Attendance>> GetAttendancesByUser(
        int userId,
        [Service] AttendanceService attendanceService)
    {
        return await attendanceService.GetAttendancesByUserIdAsync(userId);
    }

    [GraphQLDescription("Lấy chấm công theo ngày")]
    public async Task<List<Attendance>> GetAttendancesByDate(
        DateTime date,
        [Service] AttendanceService attendanceService)
    {
        return await attendanceService.GetAttendancesByDateAsync(date);
    }
}