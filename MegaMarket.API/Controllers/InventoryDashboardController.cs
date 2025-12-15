using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MegaMarket.API.Services;
using System.Security.Claims;

namespace MegaMarket.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class InventoryDashboardController : ControllerBase
{
    private readonly DashboardInventoryService _service;

    public InventoryDashboardController(DashboardInventoryService service)
    {
        _service = service;
    }

    // GET: api/InventoryDashboard
    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Warehouse")]
    public async Task<IActionResult> GetInventoryDashboard()
    {
        try
        {
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var result = await _service.GetInventoryDashboardAsync(currentRole);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to load inventory data: {ex.Message}" });
        }
    }

    // GET: api/InventoryDashboard/low-stock?limit=10
    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockProducts([FromQuery] int limit = 10)
    {
        try
        {
            var result = await _service.GetLowStockProductsAsync(limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // GET: api/InventoryDashboard/expiring?daysThreshold=30&limit=10
    [HttpGet("expiring")]
    public async Task<IActionResult> GetExpiringProducts(
        [FromQuery] int daysThreshold = 30,
        [FromQuery] int limit = 10)
    {
        try
        {
            var result = await _service.GetExpiringProductsAsync(daysThreshold, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // GET: api/InventoryDashboard/stock-by-category
    [HttpGet("stock-by-category")]
    public async Task<IActionResult> GetStockByCategory()
    {
        try
        {
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var result = await _service.GetStockByCategoryAsync(currentRole);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // GET: api/InventoryDashboard/top-moving?limit=10
    [HttpGet("top-moving")]
    public async Task<IActionResult> GetTopMovingProducts([FromQuery] int limit = 10)
    {
        try
        {
            var result = await _service.GetTopMovingProductsAsync(limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
