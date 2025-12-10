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

    /// <summary>
    /// GET /api/customerrewards?status=Pending&customerId=3
    /// Lấy danh sách tất cả rewards với tùy chọn lọc theo status hoặc customerId
    /// Dùng cho: Reports, Admin dashboard
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] int? customerId)
    {
        var result = await _customerRewardService.GetAllAsync(status, customerId);

        if (!result.Any())
            return NotFound("No customer rewards found.");

        return Ok(result);
    }

    /// <summary>
    /// GET /api/customerrewards/customer/{id}
    /// Lấy tất cả rewards của 1 customer
    /// Dùng cho: Loyalty Dashboard - hiển thị "My Rewards" section
    /// </summary>
    [HttpGet("customer/{id}")]
    public async Task<IActionResult> GetByCustomer(int id)
    {
        var result = await _customerRewardService.GetByCustomerIdAsync(id);

        if (!result.Any())
            return NotFound("This customer has no reward history or does not exist.");

        return Ok(result);
    }

    /// <summary>
    /// POST /api/customerrewards/redeem
    /// ① Customer chọn reward + click "Redeem Now"
    /// ② Backend thực hiện (transaction):
    ///    - Trừ points từ customer
    ///    - Giảm quantity_available của reward
    ///    - Tạo CustomerReward với Status = "Pending"
    ///    - Tạo PointTransaction (type='Redeem')
    /// ③ Frontend: Hiển thị reward ở "My Rewards" với Status=Pending
    /// ④ Next step: Click "Claim" để chuyển sang Claimed/Used
    /// </summary>
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

    /// <summary>
    /// PUT /api/customerrewards/{id}/claim
    /// ① Staff/Admin xác nhận khách nhân được reward
    /// ② Logic dựa theo loại reward:
    ///    - 🎁 GIFT (vật lý): Pending → tự động → Used (vì đã nhận vật)
    ///    - 🎟️ VOUCHER/DISCOUNT: Pending → Claimed (chờ áp dụng vào invoice)
    /// ③ Frontend: Refresh data, button Claim mất, hiện Apply to Invoice (chỉ Voucher)
    /// Dùng cho: Loyalty Dashboard - "My Rewards" section
    /// </summary>
    [HttpPut("{id}/claim")]
    public async Task<IActionResult> ClaimReward(int id)
    {
        try
        {
            var result = await _customerRewardService.ClaimRewardAsync(id);

            if (result == null)
                return NotFound(new { message = "Reward redemption not found." });

            return Ok(new { 
                message = $"Reward claimed successfully. Status: {result.Status}",
                data = result 
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// PUT /api/customerrewards/{id}/use
    /// ⚠️ KHÔNG DÙNG TRONG NORMAL FLOW - Giữ lại cho edge cases
    /// Thay vào đó dùng: PUT /api/customerrewards/{id}/apply-to-invoice
    /// 
    /// Điều kiện:
    /// - Status phải = "Claimed"
    /// - Voucher/Discount phải có InvoiceId
    /// 
    /// Result: Claimed → Used + UsedAt = DateTime.Now
    /// </summary>
    [HttpPut("{id}/use")]
    public async Task<IActionResult> UseReward(int id, [FromBody] UseRewardRequestDto? request)
    {
        try
        {
            var result = await _customerRewardService.UseRewardAsync(id);

            if (result == null)
                return NotFound(new { message = "Reward redemption not found." });

            return Ok(new { 
                message = "Reward used successfully.",
                data = result 
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// PUT /api/customerrewards/{id}/apply-to-invoice
    /// ① Checkout/Payment module gọi endpoint này
    /// ② Áp dụng voucher vào invoice:
    ///    - Update InvoiceId (tham chiếu đến invoice)
    ///    - Update Status: Claimed → Used
    ///    - Update UsedAt = DateTime.Now
    /// ③ Voucher giờ đã được áp dụng, có thể tính discount vào total invoice
    /// ④ Frontend: Button "Apply to Invoice" mất, hiện Status=Used
    /// 
    /// Request body: { "invoiceId": 123 }
    /// Dùng cho: Invoice/Checkout module
    /// </summary>
    [HttpPut("{id}/apply-to-invoice")]
    public async Task<IActionResult> ApplyVoucherToInvoice(int id, [FromBody] UseRewardRequestDto request)
    {
        try
        {
            if (request?.InvoiceId == null || request.InvoiceId <= 0)
                return BadRequest(new { message = "Invalid invoice ID." });

            var result = await _customerRewardService.ApplyVoucherToInvoiceAsync(id, request.InvoiceId.Value);

            if (result == null)
                return NotFound(new { message = "Reward redemption not found." });

            return Ok(new { 
                message = "✅ Voucher applied to invoice successfully!",
                data = result 
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE /api/customerrewards/{id}
    // ⚠️ ENDPOINT NÀY CHƯA ĐƯỢC IMPLEMENT - DÙNG TRONG FUTURE NẾU CẦN
    // Mục đích: Xóa reward + hoàn lại points cho customer
    // Status phải = "Pending" mới được xóa
}
