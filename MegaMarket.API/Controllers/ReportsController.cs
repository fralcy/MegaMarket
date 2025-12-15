using Microsoft.AspNetCore.Http;
using MegaMarket.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MegaMarket.API.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET /api/reports/customers/top?limit=10
        [HttpGet("customers/top")]
        public async Task<IActionResult> GetTopCustomers([FromQuery] int limit = 10)
        {
            try
            {
                var result = await _reportService.GetTopCustomersAsync(limit);
                if (!result.Any())
                    return NotFound(new { message = "No customers found." });
                return Ok(new { data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/reports/rewards/top?limit=10
        [HttpGet("rewards/top")]
        public async Task<IActionResult> GetTopRewards([FromQuery] int limit = 10)
        {
            try
            {
                var result = await _reportService.GetTopRewardsAsync(limit);
                if (!result.Any())
                    return NotFound(new { message = "No rewards found." });
                return Ok(new { data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/reports/points/summary
        [HttpGet("points/summary")]
        public async Task<IActionResult> GetPointsSummary()
        {
            try
            {
                var result = await _reportService.GetPointsSummaryAsync();
                if (!result.Any())
                    return NotFound(new { message = "No points data found." });
                return Ok(new { data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
