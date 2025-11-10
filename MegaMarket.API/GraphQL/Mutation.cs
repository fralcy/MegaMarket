using MegaMarket.Data.Models;
using MegaMarket.API.Services;
using MegaMarket.API.DTOs;

namespace MegaMarket.API.GraphQL;

public class Mutation
{
    // ==================== USER MUTATIONS ====================

    [GraphQLDescription("Tạo nhân viên mới")]
    public async Task<User> CreateUser(
        UserInputDto input,
        [Service] UserService userService)
    {
        return await userService.CreateUserAsync(input);
    }

    [GraphQLDescription("Cập nhật thông tin nhân viên")]
    public async Task<User> UpdateUser(
        int userId,
        UserInputDto input,
        [Service] UserService userService)
    {
        return await userService.UpdateUserAsync(userId, input);
    }

    [GraphQLDescription("Xóa nhân viên")]
    public async Task<bool> DeleteUser(
        int userId,
        [Service] UserService userService)
    {
        return await userService.DeleteUserAsync(userId);
    }

    // ==================== SHIFT TYPE MUTATIONS ====================

    [GraphQLDescription("Tạo loại ca làm việc mới")]
    public async Task<ShiftType> CreateShiftType(
        ShiftTypeInputDto input,
        [Service] ShiftTypeService shiftTypeService)
    {
        return await shiftTypeService.CreateShiftTypeAsync(input);
    }

    [GraphQLDescription("Cập nhật loại ca làm việc")]
    public async Task<ShiftType> UpdateShiftType(
        int shiftTypeId,
        ShiftTypeInputDto input,
        [Service] ShiftTypeService shiftTypeService)
    {
        return await shiftTypeService.UpdateShiftTypeAsync(shiftTypeId, input);
    }

    [GraphQLDescription("Xóa loại ca làm việc")]
    public async Task<bool> DeleteShiftType(
        int shiftTypeId,
        [Service] ShiftTypeService shiftTypeService)
    {
        return await shiftTypeService.DeleteShiftTypeAsync(shiftTypeId);
    }

    // ==================== ATTENDANCE MUTATIONS ====================

    [GraphQLDescription("Tạo bản ghi chấm công")]
    public async Task<Attendance> CreateAttendance(
        AttendanceInputDto input,
        [Service] AttendanceService attendanceService)
    {
        return await attendanceService.CreateAttendanceAsync(input);
    }

    [GraphQLDescription("Check-in cho nhân viên")]
    public async Task<Attendance> CheckIn(
        int userId,
        DateTime date,
        [Service] AttendanceService attendanceService)
    {
        return await attendanceService.CheckInAsync(userId, date);
    }

    [GraphQLDescription("Check-out cho nhân viên")]
    public async Task<Attendance> CheckOut(
        int userId,
        DateTime date,
        [Service] AttendanceService attendanceService)
    {
        return await attendanceService.CheckOutAsync(userId, date);
    }

    [GraphQLDescription("Cập nhật bản ghi chấm công")]
    public async Task<Attendance> UpdateAttendance(
        int attendanceId,
        AttendanceInputDto input,
        [Service] AttendanceService attendanceService)
    {
        return await attendanceService.UpdateAttendanceAsync(attendanceId, input);
    }

    [GraphQLDescription("Xóa bản ghi chấm công")]
    public async Task<bool> DeleteAttendance(
        int attendanceId,
        [Service] AttendanceService attendanceService)
    {
        return await attendanceService.DeleteAttendanceAsync(attendanceId);
    }
}