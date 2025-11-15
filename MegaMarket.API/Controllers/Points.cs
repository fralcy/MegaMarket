using MegaMarket.API.DTOs.PointTransaction;
using MegaMarket.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MegaMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Points : ControllerBase
    {
        private readonly IPointTransactionService _pointTransactionService;

        public Points(IPointTransactionService pointTransactionService)
        {
            _pointTransactionService = pointTransactionService;
        }

        // GET: api/points : get all point transactions
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FilterPointTransactionRequestDto filter)
        {
            var results = await _pointTransactionService.GetFilteredPointTransactionsAsync(filter);

            if (!results.Any())
                return NotFound("No point transactions found.");

            return Ok(results);
        }

        // update ( add ) point for customer
        [HttpPost("{id}/add")]
        public async Task<IActionResult> AddPoints(int id, [FromBody] AddPointRequestDto dto)
        {
            if (dto.Points <= 0)
                return BadRequest("Points must be greater than 0.");

            var result = await _pointTransactionService.AddPointAsync(id, dto);

            if (result == null)
                return NotFound("Customer not found.");

            return Ok(result);
        }


        //api/points/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPointTransactionById(int id)
        {
            var point_transaction = await _pointTransactionService.GetPointTransactionAsyncById(id);
            if (point_transaction == null)
            {
                return NotFound();
            }
            return Ok(point_transaction);
        }

        [HttpPost("{id}/subtract")]
        public async Task<IActionResult> SubtractPoints(int id, [FromBody] SubtractPointRequestDto dto)
        {
            try
            {
                var result = await _pointTransactionService.SubtractPointAsync(id, dto);

                if (result == null)
                    return NotFound("Customer not found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
