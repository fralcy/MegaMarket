using MegaMarket.API.DTOs.CustomerRewards;
using MegaMarket.API.Services;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class CustomerRewardsController : ControllerBase
{
    private readonly ICustomerRewardService _customerRewardService;

    public CustomerRewardsController(ICustomerRewardService customerRewardService)
    {
        _customerRewardService = customerRewardService;
    }

    // GET /api/customerrewards?status=Pending&customerId=3
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] int? customerId)
    {
        var result = await _customerRewardService.GetAllAsync(status, customerId);

        if (!result.Any())
            return NotFound("No customer rewards found.");

        return Ok(result);
    }

    // GET /api/customerrewards/customer/{id}
    [HttpGet("customer/{id}")]
    public async Task<IActionResult> GetByCustomer(int id)
    {
        var result = await _customerRewardService.GetByCustomerIdAsync(id);

        if (!result.Any())
            return NotFound("This customer has no reward history or does not exist.");

        return Ok(result);
    }

    // PUT /api/customerrewards/{id}/use
    [HttpPut("{id}/use")]
    public async Task<IActionResult> UseReward(int id)
    {
        try
        {
            var result = await _customerRewardService.UseRewardAsync(id);

            if (result == null)
                return NotFound("Reward redemption not found.");

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE /api/customerrewards/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReward(int id)
    {
        try
        {
            await _customerRewardService.DeleteCustomerRewardAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST /api/customerrewards/redeem
    [HttpPost("redeem")]
    public async Task<IActionResult> RedeemReward([FromBody] RedeemRewardRequestDto request)
    {
        try
        {
            // Validate request
            if (request == null)
                return BadRequest(new { message = "Request body is required." });

            if (request.CustomerId <= 0)
                return BadRequest(new { message = "Invalid customer ID." });

            if (request.RewardId <= 0)
                return BadRequest(new { message = "Invalid reward ID." });

            // Call service to redeem reward
            var result = await _customerRewardService.RedeemRewardAsync(
                customerId: request.CustomerId,
                rewardId: request.RewardId,
                invoiceId: request.InvoiceId ?? null
            );


            return Ok(new { message = "Reward redeemed successfully.", data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


}
