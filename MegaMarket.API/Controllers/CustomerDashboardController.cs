using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MegaMarket.API.Services;
using MegaMarket.API.DTOs.Dashboard.Common;
using System.Security.Claims;

namespace MegaMarket.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CustomerDashboardController : ControllerBase
{
    private readonly DashboardCustomerService _service;

    public CustomerDashboardController(DashboardCustomerService service)
    {
        _service = service;
    }

    // GET: api/CustomerDashboard?dateRange=TODAY
    [HttpGet]
    public async Task<IActionResult> GetCustomerDashboard([FromQuery] string dateRange = "TODAY")
    {
        try
        {
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!Enum.TryParse<DateRangeEnum>(dateRange, true, out var dateRangeEnum))
            {
                return BadRequest(new { message = "Invalid date range. Use: TODAY, THIS_WEEK, THIS_MONTH, THIS_YEAR" });
            }

            var result = await _service.GetCustomerDashboardAsync(dateRangeEnum, currentRole);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to load customer data: {ex.Message}" });
        }
    }

    // GET: api/CustomerDashboard/top-customers?dateRange=TODAY&limit=10
    [HttpGet("top-customers")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetTopCustomers(
        [FromQuery] string dateRange = "TODAY",
        [FromQuery] int limit = 10)
    {
        try
        {
            if (!Enum.TryParse<DateRangeEnum>(dateRange, true, out var dateRangeEnum))
            {
                return BadRequest(new { message = "Invalid date range" });
            }

            var result = await _service.GetTopCustomersByRevenueAsync(dateRangeEnum, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // GET: api/CustomerDashboard/rank-distribution
    [HttpGet("rank-distribution")]
    public async Task<IActionResult> GetCustomerRankDistribution()
    {
        try
        {
            var result = await _service.GetCustomerRankDistributionAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
