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

    // ==================== SALES DASHBOARD QUERIES ====================

    [Authorize]
    [GraphQLDescription("Lấy thống kê doanh thu")]
    public async Task<DTOs.Dashboard.Sales.SalesDashboardDto> GetSalesDashboard(
        DTOs.Dashboard.Common.DateRangeEnum dateRange,
        ClaimsPrincipal claimsPrincipal,
        [Service] DashboardSalesService service)
    {
        var currentUserId = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var currentRole = claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value;
        return await service.GetSalesDashboardAsync(dateRange, currentRole, currentUserId);
    }

    [Authorize(Roles = new[] { "Admin" })]
    [GraphQLDescription("Lấy xu hướng doanh thu theo ngày")]
    public async Task<List<DTOs.Dashboard.Sales.RevenueTrendDto>> GetRevenueTrend(
        DTOs.Dashboard.Common.DateRangeEnum dateRange,
        [Service] DashboardSalesService service)
    {
        return await service.GetRevenueTrendAsync(dateRange);
    }

    [Authorize]
    [GraphQLDescription("Lấy top sản phẩm bán chạy")]
    public async Task<List<DTOs.Dashboard.Sales.TopProductDto>> GetTopSellingProducts(
        DTOs.Dashboard.Common.DateRangeEnum dateRange,
        int limit,
        ClaimsPrincipal claimsPrincipal,
        [Service] DashboardSalesService service)
    {
        var currentRole = claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value;
        return await service.GetTopSellingProductsAsync(dateRange, limit, currentRole);
    }

    // ==================== INVENTORY DASHBOARD QUERIES ====================

    [Authorize]
    [GraphQLDescription("Lấy thống kê tồn kho")]
    public async Task<DTOs.Dashboard.Inventory.InventoryDashboardDto> GetInventoryDashboard(
        ClaimsPrincipal claimsPrincipal,
        [Service] DashboardInventoryService service)
    {
        var currentRole = claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value;
        return await service.GetInventoryDashboardAsync(currentRole);
    }

    [Authorize]
    [GraphQLDescription("Lấy sản phẩm sắp hết hàng")]
    public async Task<List<DTOs.Dashboard.Inventory.LowStockProductDto>> GetLowStockProducts(
        int limit,
        [Service] DashboardInventoryService service)
    {
        return await service.GetLowStockProductsAsync(limit);
    }

    [Authorize]
    [GraphQLDescription("Lấy sản phẩm sắp hết hạn")]
    public async Task<List<DTOs.Dashboard.Inventory.ExpiringProductDto>> GetExpiringProducts(
        int daysThreshold,
        int limit,
        [Service] DashboardInventoryService service)
    {
        return await service.GetExpiringProductsAsync(daysThreshold, limit);
    }

    [Authorize]
    [GraphQLDescription("Lấy thống kê tồn kho theo danh mục")]
    public async Task<List<DTOs.Dashboard.Inventory.CategoryStockDto>> GetStockByCategory(
        ClaimsPrincipal claimsPrincipal,
        [Service] DashboardInventoryService service)
    {
        var currentRole = claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value;
        return await service.GetStockByCategoryAsync(currentRole);
    }

    [Authorize]
    [GraphQLDescription("Lấy top sản phẩm bán chạy (30 ngày)")]
    public async Task<List<DTOs.Dashboard.Inventory.TopMovingProductDto>> GetTopMovingProducts(
        int limit,
        [Service] DashboardInventoryService service)
    {
        return await service.GetTopMovingProductsAsync(limit);
    }
}