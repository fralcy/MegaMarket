using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MegaMarket.API.Services;
using MegaMarket.API.DTOs.Dashboard.Common;
using System.Security.Claims;

namespace MegaMarket.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SalesDashboardController : ControllerBase
{
    private readonly DashboardSalesService _service;

    public SalesDashboardController(DashboardSalesService service)
    {
        _service = service;
    }

    // GET: api/SalesDashboard?dateRange=TODAY
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetSalesDashboard([FromQuery] string dateRange = "TODAY")
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!Enum.TryParse<DateRangeEnum>(dateRange, true, out var dateRangeEnum))
            {
                return BadRequest(new { message = "Invalid date range. Use: TODAY, THIS_WEEK, THIS_MONTH, THIS_YEAR" });
            }

            var result = await _service.GetSalesDashboardAsync(dateRangeEnum, currentRole, currentUserId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to load sales data: {ex.Message}" });
        }
    }

    // GET: api/SalesDashboard/revenue-trend?dateRange=TODAY
    [HttpGet("revenue-trend")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetRevenueTrend([FromQuery] string dateRange = "TODAY")
    {
        try
        {
            if (!Enum.TryParse<DateRangeEnum>(dateRange, true, out var dateRangeEnum))
            {
                return BadRequest(new { message = "Invalid date range" });
            }

            var result = await _service.GetRevenueTrendAsync(dateRangeEnum);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // GET: api/SalesDashboard/top-products?dateRange=TODAY&limit=10
    [HttpGet("top-products")]
    public async Task<IActionResult> GetTopSellingProducts(
        [FromQuery] string dateRange = "TODAY",
        [FromQuery] int limit = 10)
    {
        try
        {
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!Enum.TryParse<DateRangeEnum>(dateRange, true, out var dateRangeEnum))
            {
                return BadRequest(new { message = "Invalid date range" });
            }

            var result = await _service.GetTopSellingProductsAsync(dateRangeEnum, limit, currentRole);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
