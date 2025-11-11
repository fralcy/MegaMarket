using MegaMarket.Data.Models;
using MegaMarket.API.Services;
using HotChocolate.Authorization;
using System.Security.Claims;

namespace MegaMarket.API.GraphQL;

public class Query
{
    // ==================== AUTH QUERIES ====================

    [Authorize] // Phải login
    [GraphQLDescription("Lấy thông tin user hiện tại đang login")]
    public async Task<User?> GetCurrentUser(
        ClaimsPrincipal claimsPrincipal,
        [Service] UserService userService)
    {
        var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return null;
        }

        var userId = int.Parse(userIdClaim.Value);
        return await userService.GetUserByIdAsync(userId);
    }

    // ==================== USER QUERIES ====================

    [Authorize(Roles = new[] { "Admin" })] // Chỉ Admin xem được list
    [GraphQLDescription("Lấy tất cả nhân viên")]
    public async Task<List<User>> GetUsers([Service] UserService userService)
    {
        return await userService.GetAllUsersAsync();
    }

    [Authorize] // Cả Admin và Employee đều xem được
    [GraphQLDescription("Lấy nhân viên theo ID")]
    public async Task<User?> GetUser(
        int userId,
        [Service] UserService userService)
    {
        return await userService.GetUserByIdAsync(userId);
    }

    [Authorize] // Cả Admin và Employee đều xem được
    [GraphQLDescription("Lấy nhân viên theo username")]
    public async Task<User?> GetUserByUsername(
        string username,
        [Service] UserService userService)
    {
        return await userService.GetUserByUsernameAsync(username);
    }

    // ==================== SHIFT TYPE QUERIES ====================

    [Authorize] // Phải login
    [GraphQLDescription("Lấy tất cả loại ca làm việc")]
    public async Task<List<ShiftType>> GetShiftTypes([Service] ShiftTypeService shiftTypeService)
    {
        return await shiftTypeService.GetAllShiftTypesAsync();
    }

    [Authorize] // Phải login
    [GraphQLDescription("Lấy loại ca theo ID")]
    public async Task<ShiftType?> GetShiftType(
        int shiftTypeId,
        [Service] ShiftTypeService shiftTypeService)
    {
        return await shiftTypeService.GetShiftTypeByIdAsync(shiftTypeId);
    }

    [Authorize] // Phải login
    [GraphQLDescription("Lấy loại ca theo tên")]
    public async Task<ShiftType?> GetShiftTypeByName(
        string name,
        [Service] ShiftTypeService shiftTypeService)
    {
        return await shiftTypeService.GetShiftTypeByNameAsync(name);
    }

    // ==================== ATTENDANCE QUERIES ====================

    [Authorize(Roles = new[] { "Admin" })] // Chỉ Admin xem được tất cả
    [GraphQLDescription("Lấy tất cả bản ghi chấm công")]
    public async Task<List<Attendance>> GetAttendances([Service] AttendanceService attendanceService)
    {
        return await attendanceService.GetAllAttendancesAsync();
    }

    [Authorize] // Phải login
    [GraphQLDescription("Lấy chấm công theo ID")]
    public async Task<Attendance?> GetAttendance(
        int attendanceId,
        [Service] AttendanceService attendanceService)
    {
        return await attendanceService.GetAttendanceByIdAsync(attendanceId);
    }

    [Authorize] // Phải login
    [GraphQLDescription("Lấy lịch sử chấm công của nhân viên")]
    public async Task<List<Attendance>> GetAttendancesByUser(
        int userId,
        ClaimsPrincipal claimsPrincipal,
        [Service] AttendanceService attendanceService)
    {
        // Employee Chỉ xem được của chính mình
        // Admin xem được của bất kỳ ai
        var currentUserIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
        var currentRole = claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value;

        if (currentRole != "Admin" && currentUserIdClaim?.Value != userId.ToString())
        {
            throw new GraphQLException("Bạn không có quyền xem attendance của người khác!");
        }

        return await attendanceService.GetAttendancesByUserIdAsync(userId);
    }

    [Authorize] // Phải login
    [GraphQLDescription("Lấy chấm công theo ngày")]
    public async Task<List<Attendance>> GetAttendancesByDate(
        DateTime date,
        [Service] AttendanceService attendanceService)
    {
        return await attendanceService.GetAttendancesByDateAsync(date);
    }
}