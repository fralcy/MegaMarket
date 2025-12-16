using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MegaMarket.API.Services;
using MegaMarket.API.DTOs.Dashboard.Common;
using System.Security.Claims;

namespace MegaMarket.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EmployeeDashboardController : ControllerBase
{
    private readonly DashboardEmployeeService _service;

    public EmployeeDashboardController(DashboardEmployeeService service)
    {
        _service = service;
    }

    // GET: api/EmployeeDashboard?dateRange=TODAY
    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Cashier,Warehouse")]
    public async Task<IActionResult> GetEmployeeDashboard([FromQuery] string dateRange = "TODAY")
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!Enum.TryParse<DateRangeEnum>(dateRange, true, out var dateRangeEnum))
            {
                return BadRequest(new { message = "Invalid date range. Use: TODAY, THIS_WEEK, THIS_MONTH, THIS_YEAR" });
            }

            var result = await _service.GetEmployeeDashboardAsync(dateRangeEnum, currentRole, currentUserId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to load employee dashboard: {ex.Message}" });
        }
    }

    // GET: api/EmployeeDashboard/work-summary?dateRange=TODAY
    [HttpGet("work-summary")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetEmployeeWorkSummary([FromQuery] string dateRange = "TODAY")
    {
        try
        {
            if (!Enum.TryParse<DateRangeEnum>(dateRange, true, out var dateRangeEnum))
            {
                return BadRequest(new { message = "Invalid date range. Use: TODAY, THIS_WEEK, THIS_MONTH, THIS_YEAR" });
            }

            var result = await _service.GetEmployeeWorkSummaryAsync(dateRangeEnum);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to load employee work summary: {ex.Message}" });
        }
    }

    // GET: api/EmployeeDashboard/attendance-statistics/{employeeId}?dateRange=TODAY
    [HttpGet("attendance-statistics/{employeeId}")]
    public async Task<IActionResult> GetAttendanceStatistics(int employeeId, [FromQuery] string dateRange = "TODAY")
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!Enum.TryParse<DateRangeEnum>(dateRange, true, out var dateRangeEnum))
            {
                return BadRequest(new { message = "Invalid date range. Use: TODAY, THIS_WEEK, THIS_MONTH, THIS_YEAR" });
            }

            // Access control: Admin and Manager can see all employees, Employee can only see themselves
            if (currentRole != "Admin" && currentRole != "Manager" && employeeId != currentUserId)
            {
                return Forbid();
            }

            var result = await _service.GetAttendanceStatisticsAsync(employeeId, dateRangeEnum);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to load attendance statistics: {ex.Message}" });
        }
    }
}
