using MegaMarket.Data.Models;
using MegaMarket.API.Services;
using MegaMarket.API.DTOs;
using HotChocolate.Authorization;

namespace MegaMarket.API.GraphQL;

public class Mutation
{
    // ==================== AUTH MUTATIONS ====================

    [GraphQLDescription("Đăng nhập vào hệ thống")]
    public async Task<LoginResultDto?> Login(
        LoginInputDto input,
        [Service] AuthService authService)
    {
        var result = await authService.LoginAsync(input);

        if (result == null)
        {
            throw new GraphQLException("Tên đăng nhập hoặc mật khẩu không đúng!");
        }

        return result;
    }

    // ==================== USER MUTATIONS ====================

    [Authorize(Roles = new[] { "Admin" })] // Chỉ Admin mới tạo được user
    [GraphQLDescription("Tạo nhân viên mới")]
    public async Task<User> CreateUser(
        UserInputDto input,
        [Service] UserService userService)
    {
        return await userService.CreateUserAsync(input);
    }

    [Authorize(Roles = new[] { "Admin" })] // Chỉ Admin
    [GraphQLDescription("Cập nhật thông tin nhân viên")]
    public async Task<User> UpdateUser(
        int userId,
        UserInputDto input,
        [Service] UserService userService)
    {
        return await userService.UpdateUserAsync(userId, input);
    }

    [Authorize(Roles = new[] { "Admin" })] // Chỉ Admin
    [GraphQLDescription("Xóa nhân viên")]
    public async Task<bool> DeleteUser(
        int userId,
        [Service] UserService userService)
    {
        return await userService.DeleteUserAsync(userId);
    }

    // ==================== SHIFT TYPE MUTATIONS ====================

    [Authorize(Roles = new[] { "Admin" })] // Chỉ Admin
    [GraphQLDescription("Tạo loại ca làm việc mới")]
    public async Task<ShiftType> CreateShiftType(
        ShiftTypeInputDto input,
        [Service] ShiftTypeService shiftTypeService)
    {
        return await shiftTypeService.CreateShiftTypeAsync(input);
    }

    [Authorize(Roles = new[] { "Admin" })] // Chỉ Admin
    [GraphQLDescription("Cập nhật loại ca làm việc")]
    public async Task<ShiftType> UpdateShiftType(
        int shiftTypeId,
        ShiftTypeInputDto input,
        [Service] ShiftTypeService shiftTypeService)
    {
        return await shiftTypeService.UpdateShiftTypeAsync(shiftTypeId, input);
    }

    [Authorize(Roles = new[] { "Admin" })] // Chỉ Admin
    [GraphQLDescription("Xóa loại ca làm việc")]
    public async Task<bool> DeleteShiftType(
        int shiftTypeId,
        [Service] ShiftTypeService shiftTypeService)
    {
        return await shiftTypeService.DeleteShiftTypeAsync(shiftTypeId);
    }

    // ==================== ATTENDANCE MUTATIONS ====================

    [Authorize] // Cả Admin và Employee đều được
    [GraphQLDescription("Tạo bản ghi chấm công")]
    public async Task<Attendance> CreateAttendance(
        AttendanceInputDto input,
        [Service] AttendanceService attendanceService)
    {
        return await attendanceService.CreateAttendanceAsync(input);
    }

    [Authorize] // Cả Admin và Employee đều được check-in
    [GraphQLDescription("Check-in cho nhân viên")]
    public async Task<Attendance> CheckIn(
        int userId,
        DateTime date,
        [Service] AttendanceService attendanceService)
    {
        return await attendanceService.CheckInAsync(userId, date);
    }

    [Authorize] // Cả Admin và Employee đều được check-out
    [GraphQLDescription("Check-out cho nhân viên")]
    public async Task<Attendance> CheckOut(
        int userId,
        DateTime date,
        [Service] AttendanceService attendanceService)
    {
        return await attendanceService.CheckOutAsync(userId, date);
    }

    [Authorize(Roles = new[] { "Admin" })] // Chỉ Admin sửa được
    [GraphQLDescription("Cập nhật bản ghi chấm công")]
    public async Task<Attendance> UpdateAttendance(
        int attendanceId,
        AttendanceInputDto input,
        [Service] AttendanceService attendanceService)
    {
        return await attendanceService.UpdateAttendanceAsync(attendanceId, input);
    }

    [Authorize(Roles = new[] { "Admin" })] // Chỉ Admin xóa được
    [GraphQLDescription("Xóa bản ghi chấm công")]
    public async Task<bool> DeleteAttendance(
        int attendanceId,
        [Service] AttendanceService attendanceService)
    {
        return await attendanceService.DeleteAttendanceAsync(attendanceId);
    }
}