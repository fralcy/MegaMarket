using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MegaMarket.API.Services;
using MegaMarket.API.DTOs;
using System.Security.Claims;

namespace MegaMarket.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AttendanceController : ControllerBase
{
    private readonly AttendanceService _attendanceService;

    public AttendanceController(AttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    // GET: api/Attendance
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAttendances()
    {
        try
        {
            var attendances = await _attendanceService.GetAllAttendancesAsync();
            return Ok(attendances);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to load attendances: {ex.Message}" });
        }
    }

    // GET: api/Attendance/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAttendance(int id)
    {
        try
        {
            var attendance = await _attendanceService.GetAttendanceByIdAsync(id);

            if (attendance == null)
            {
                return NotFound(new { message = "Attendance not found" });
            }

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Access control: Admin and Manager can see all, Employee can only see their own
            if (currentRole != "Admin" && currentRole != "Manager" && attendance.UserId != currentUserId)
            {
                return Forbid();
            }

            return Ok(attendance);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to load attendance: {ex.Message}" });
        }
    }

    // GET: api/Attendance/user/5
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetAttendancesByUser(int userId)
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Access control: Admin and Manager can see all, Employee can only see their own
            if (currentRole != "Admin" && currentRole != "Manager" && currentUserId != userId)
            {
                return Forbid();
            }

            var attendances = await _attendanceService.GetAttendancesByUserIdAsync(userId);
            return Ok(attendances);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to load attendances: {ex.Message}" });
        }
    }

    // GET: api/Attendance/date/2024-01-15
    [HttpGet("date/{date}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAttendancesByDate(DateTime date)
    {
        try
        {
            var attendances = await _attendanceService.GetAttendancesByDateAsync(date);
            return Ok(attendances);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to load attendances: {ex.Message}" });
        }
    }

    // POST: api/Attendance
    [HttpPost]
    public async Task<IActionResult> CreateAttendance([FromBody] AttendanceInputDto input)
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Access control: Admin and Manager can create for anyone, Employee can only create for themselves
            if (currentRole != "Admin" && currentRole != "Manager" && input.UserId != currentUserId)
            {
                return Forbid();
            }

            var attendance = await _attendanceService.CreateAttendanceAsync(input);
            return CreatedAtAction(nameof(GetAttendance), new { id = attendance.AttendanceId }, attendance);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/Attendance/check-in
    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInOutDto input)
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Access control: Admin and Manager can check-in for anyone, Employee can only check-in for themselves
            if (currentRole != "Admin" && currentRole != "Manager" && input.UserId != currentUserId)
            {
                return Forbid();
            }

            var attendance = await _attendanceService.CheckInAsync(input.UserId, input.Date);
            return Ok(attendance);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/Attendance/check-out
    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut([FromBody] CheckInOutDto input)
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Access control: Admin and Manager can check-out for anyone, Employee can only check-out for themselves
            if (currentRole != "Admin" && currentRole != "Manager" && input.UserId != currentUserId)
            {
                return Forbid();
            }

            var attendance = await _attendanceService.CheckOutAsync(input.UserId, input.Date);
            return Ok(attendance);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/Attendance/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateAttendance(int id, [FromBody] AttendanceInputDto input)
    {
        try
        {
            var attendance = await _attendanceService.UpdateAttendanceAsync(id, input);
            return Ok(attendance);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/Attendance/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteAttendance(int id)
    {
        try
        {
            var result = await _attendanceService.DeleteAttendanceAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Attendance not found" });
            }

            return Ok(new { message = "Attendance deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

// DTO for check-in/check-out operations
public class CheckInOutDto
{
    public int UserId { get; set; }
    public DateTime Date { get; set; }
}
