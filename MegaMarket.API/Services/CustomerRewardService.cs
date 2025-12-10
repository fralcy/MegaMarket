using MegaMarket.API.DTOs.CustomerRewards;
using MegaMarket.Data.Models;
using MegaMarket.Data.Repositories;

/// <summary>
/// CustomerRewardService - Xử lý logic liên quan đến customer redemptions (đổi quà)
/// 
/// FLOW TỔNG QUÁT:
/// 1. REDEEM: Customer chọn quà, backend trừ points + giảm quantity → Status=Pending
/// 2. CLAIM: Staff/Admin xác nhận → Gift:Used, Voucher:Claimed
/// 3. USE/APPLY: Voucher áp dụng vào invoice → Status=Used
/// 
/// ⚠️ LƯU Ý VỀ LOGIC:
/// - RedeemRewardAsync: Sử dụng transaction đảm bảo atomicity
/// - ClaimRewardAsync: Tự động phân biệt Gift vs Voucher
/// - ApplyVoucherToInvoiceAsync: CHÍNH là flow từ Checkout (không dùng UseRewardAsync)
/// - UseRewardAsync: Edge case, nếu cần update manual từ Dashboard
/// </summary>
public class CustomerRewardService : ICustomerRewardService
{
    private readonly ICustomerRewardRepository _customerRewardRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly IRewardRepository _rewardRepo;

    public CustomerRewardService(ICustomerRewardRepository customerRewardRepo, ICustomerRepository customerRepo, IRewardRepository rewardRepo)
    {
        _customerRewardRepo = customerRewardRepo;
        _customerRepo = customerRepo;
        _rewardRepo = rewardRepo;
    }

    /// <summary>
    /// Lấy tất cả customer rewards với optional filter
    /// Dùng cho: Reports, Admin dashboard
    /// </summary>
    public async Task<IEnumerable<CustomerRewardResponseDto>> GetAllAsync(string? status, int? customerId)
    {
        // get data from db
        var data = await _customerRewardRepo.GetAllAsync(status, customerId);

        // map to dto and return
        return data.Select(cr => new CustomerRewardResponseDto
        {
            RedemptionId = cr.RedemptionId,
            CustomerId = cr.CustomerId,
            CustomerName = cr.Customer?.FullName ?? "",
            RewardId = cr.RewardId,
            RewardName = cr.Reward?.Name ?? "",
            InvoiceId = cr.InvoiceId,
            RedeemedAt = cr.RedeemedAt,
            Status = cr.Status,
            UsedAt = cr.UsedAt
        });
    }

    /// <summary>
    /// Lấy tất cả rewards của 1 customer
    /// Dùng cho: Loyalty Dashboard - "My Rewards" section
    /// </summary>
    public async Task<IEnumerable<CustomerRewardResponseDto>> GetByCustomerIdAsync(int customerId)
    {
        // Check customer exists
        var customerExists = await _customerRepo.GetCustomerByIdAsync(customerId);
        if (customerExists == null)
            return Enumerable.Empty<CustomerRewardResponseDto>();

        // get data from db
        var list = await _customerRewardRepo.GetByCustomerIdAsync(customerId);

        return list.Select(cr => new CustomerRewardResponseDto
        {
            RedemptionId = cr.RedemptionId,
            CustomerId = cr.CustomerId,
            CustomerName = cr.Customer?.FullName ?? "",
            RewardId = cr.RewardId,
            RewardName = cr.Reward?.Name ?? "",
            InvoiceId = cr.InvoiceId,
            RedeemedAt = cr.RedeemedAt,
            Status = cr.Status,
            UsedAt = cr.UsedAt
        });

    }

    /// <summary>
    /// REDEEM FLOW - Step 1
    /// ① Customer chọn reward + click "Redeem Now"
    /// ② Validations:
    ///    - Customer có đủ points?
    ///    - Reward còn stock?
    /// ③ Repository handle transaction:
    ///    - Trừ points từ customer
    ///    - Giảm quantity_available
    ///    - Tạo CustomerReward (Status=Pending)
    ///    - Tạo PointTransaction (type='Redeem')
    /// ④ Update customer rank sau khi trừ points
    /// Result: Status=Pending, chờ Staff claim
    /// </summary>
    public async Task<CustomerRewardResponseDto> RedeemRewardAsync(int customerId, int rewardId, int? invoiceId)
    {
        // Step 1: Validate customer exists
        var customer = await _customerRepo.GetCustomerByIdAsync(customerId);
        if (customer == null)
            throw new Exception("Customer not found.");

        // Step 2: Validate reward exists
        var reward = await _rewardRepo.GetRewardByIdAsync(rewardId);
        if (reward == null)
            throw new Exception("Reward not found.");

        // Step 3: Check sufficient points
        if (customer.Points < reward.PointCost)
            throw new Exception($"Insufficient points to redeem this reward. Required: {reward.PointCost}, Available: {customer.Points}");

        // Step 4: Check reward availability
        if (reward.QuantityAvailable <= 0)
            throw new Exception("Reward is out of stock.");

        // Step 5: Call repository to handle transaction (atomicity guaranteed)
        var customerReward = await _customerRewardRepo.RedeemRewardAsync(customerId, rewardId, invoiceId, reward.PointCost);

        // Step 6: Update rank after redeeming (có thể thay đổi rank nếu points đủ điều kiện)
        await _customerRepo.UpdateCustomerRankAsync(customerId);

        // Step 7: Return response DTO
        return new CustomerRewardResponseDto
        {
            RedemptionId = customerReward.RedemptionId,
            CustomerId = customerReward.CustomerId,
            CustomerName = customer.FullName ?? "",
            RewardId = customerReward.RewardId,
            RewardName = reward.Name ?? "",
            InvoiceId = customerReward.InvoiceId,
            RedeemedAt = customerReward.RedeemedAt,
            Status = customerReward.Status,
            UsedAt = customerReward.UsedAt
        };
    }

    /// <summary>
    /// CLAIM FLOW - Step 2 - Xác nhận nhân quà
    /// ① Staff/Admin xác nhận khách nhân được reward
    /// ② Logic phân biệt theo loại reward:
    ///    - 🎁 GIFT: Pending → Used (vật lý đã nhận, không cần bước Use)
    ///    - 🎟️ VOUCHER/DISCOUNT: Pending → Claimed (chờ áp dụng vào invoice)
    /// Result: 
    ///    - Gift: UsedAt = DateTime.Now
    ///    - Voucher: InvoiceId = null, chờ áp dụng
    /// </summary>
    public async Task<CustomerRewardResponseDto?> ClaimRewardAsync(int id)
    {
        var reward = await _customerRewardRepo.GetCustomerRewardByIdAsync(id);
        if (reward == null)
            return null;

        // only pending rewards can be claimed
        if (reward.Status != "Pending")
            throw new Exception($"Reward must be in 'Pending' status. Current status: {reward.Status}");

        // Phân biệt theo loại reward
        if (reward.Reward.RewardType == "Gift")
        {
            // Gift: tự động sang Used vì vật lý đã nhận
            reward.Status = "Used";
            reward.UsedAt = DateTime.Now;
        }
        else if (reward.Reward.RewardType == "Voucher" || reward.Reward.RewardType == "Discount")
        {
            // Voucher: sang Claimed, chờ áp dụng vào invoice
            reward.Status = "Claimed";
        }

        await _customerRewardRepo.UpdateCustomerRewardAsync(reward);

        return new CustomerRewardResponseDto
        {
            RedemptionId = reward.RedemptionId,
            CustomerId = reward.CustomerId,
            CustomerName = reward.Customer?.FullName ?? "",
            RewardId = reward.RewardId,
            RewardName = reward.Reward?.Name ?? "",
            InvoiceId = reward.InvoiceId,
            RedeemedAt = reward.RedeemedAt,
            Status = reward.Status,
            UsedAt = reward.UsedAt
        };
    }

    /// <summary>
    /// ⚠️ EDGE CASE - Không dùng trong normal flow
    /// Dùng khi: Manual update từ Dashboard (không thường xuyên)
    /// 
    /// So sánh:
    /// - UseRewardAsync: Chỉ set Status=Used (không update InvoiceId)
    /// - ApplyVoucherToInvoiceAsync: CHÍNH là flow chuẩn (set cả InvoiceId)
    /// 
    /// Điều kiện: Status=Claimed + có InvoiceId
    /// </summary>
    public async Task<CustomerRewardResponseDto?> UseRewardAsync(int id)
    {
        var reward = await _customerRewardRepo.GetCustomerRewardByIdAsync(id);
        if (reward == null)
            return null;

        // only claimed rewards can be used (dành cho Voucher)
        if (reward.Status != "Claimed")
            throw new Exception($"Reward must be in 'Claimed' status before it can be used. Current status: {reward.Status}");

        // Chỉ cho phép use Voucher/Discount có InvoiceId
        if ((reward.Reward.RewardType == "Voucher" || reward.Reward.RewardType == "Discount"))
        {
            if (reward.InvoiceId == null)
                throw new Exception("❌ Voucher must be applied to an invoice. Use ApplyVoucherToInvoiceAsync instead.");
        }

        reward.Status = "Used";
        reward.UsedAt = DateTime.Now;

        await _customerRewardRepo.UpdateCustomerRewardAsync(reward);

        return new CustomerRewardResponseDto
        {
            RedemptionId = reward.RedemptionId,
            CustomerId = reward.CustomerId,
            CustomerName = reward.Customer?.FullName ?? "",
            RewardId = reward.RewardId,
            RewardName = reward.Reward?.Name ?? "",
            InvoiceId = reward.InvoiceId,
            RedeemedAt = reward.RedeemedAt,
            Status = reward.Status,
            UsedAt = reward.UsedAt
        };
    }

    /// <summary>
    /// USE FLOW (Checkout) - Step 3 - Áp dụng Voucher vào Invoice
    /// ✅ CHÍNH LÀ ENDPOINT DÙNG TRONG CHECKOUT FLOW
    /// 
    /// ① Checkout/Payment module gọi endpoint này
    /// ② Với InvoiceId từ invoice đã tạo
    /// ③ Backend:
    ///    - Set InvoiceId = request.invoiceId
    ///    - Update Status: Claimed → Used
    ///    - Set UsedAt = DateTime.Now
    /// ④ Voucher giờ đã được áp dụng, tính discount vào invoice
    /// 
    /// Request body: { "invoiceId": 123 }
    /// </summary>
    public async Task<CustomerRewardResponseDto?> ApplyVoucherToInvoiceAsync(int redemptionId, int invoiceId)
    {
        var reward = await _customerRewardRepo.GetCustomerRewardByIdAsync(redemptionId);
        if (reward == null)
            return null;

        // Chỉ Claimed rewards mới được apply
        if (reward.Status != "Claimed")
            throw new Exception($"Only Claimed rewards can be applied. Current status: {reward.Status}");

        // Chỉ Voucher/Discount mới apply vào invoice
        if (reward.Reward.RewardType != "Voucher" && reward.Reward.RewardType != "Discount")
            throw new Exception("Only Vouchers and Discounts can be applied to invoices.");

        // Update InvoiceId + Status
        reward.InvoiceId = invoiceId;
        reward.Status = "Used";
        reward.UsedAt = DateTime.Now;

        await _customerRewardRepo.UpdateCustomerRewardAsync(reward);

        return new CustomerRewardResponseDto
        {
            RedemptionId = reward.RedemptionId,
            CustomerId = reward.CustomerId,
            CustomerName = reward.Customer?.FullName ?? "",
            RewardId = reward.RewardId,
            RewardName = reward.Reward?.Name ?? "",
            InvoiceId = reward.InvoiceId,
            RedeemedAt = reward.RedeemedAt,
            Status = reward.Status,
            UsedAt = reward.UsedAt
        };
    }

    /// <summary>
    /// DELETE FLOW - Hủy reward + Hoàn lại points
    /// Điều kiện: Status = "Pending" (chưa được claim)
    /// 
    /// Thực hiện:
    /// ① Hoàn lại points cho customer
    /// ② Update rank (có thể downgrade)
    /// ③ Tăng quantity_available của reward
    /// ④ Xóa CustomerReward record
    /// 
    /// Dùng cho: Loyalty Dashboard - Customer tự hủy reward (nếu cho phép)
    /// </summary>
    public async Task DeleteCustomerRewardAsync(int id)
    {
        var reward = await _customerRewardRepo.GetCustomerRewardByIdAsync(id);
        if (reward == null)
            throw new Exception("Customer reward not found.");
        
        // only pending rewards can be deleted
        if (reward.Status != "Pending")
            throw new Exception("Only pending rewards can be deleted.");

        // Step 1: Hoàn lại points cho customer
        var customer = await _customerRepo.GetCustomerByIdAsync(reward.CustomerId);
        if (customer != null)
        {
            customer.Points += reward.Reward.PointCost;
            await _customerRepo.UpdateCustomerAsync(customer);
            // Step 2: Update rank (có thể downgrade)
            await _customerRepo.UpdateCustomerRankAsync(customer.CustomerId);
        }

        // Step 3: Tăng quantity_available (hoàn nguyên stock)
        var rewardItem = await _rewardRepo.GetRewardByIdAsync(reward.RewardId);
        if (rewardItem != null)
        {
            rewardItem.QuantityAvailable += 1;
            await _rewardRepo.UpdateRewardAsync(rewardItem);
        }

        // Step 4: Xóa redemption record
        await _customerRewardRepo.DeleteCustomerRewardAsync(id);
    }
}
