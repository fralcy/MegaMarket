using MegaMarket.API.DTOs.CustomerRewards;
using MegaMarket.Data.Models;
using MegaMarket.Data.Repositories;

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

    // get all customer rewards with optional filtering by status and customerId
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

    // get customer rewards by customer id
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

    // use reward and update its status
    public async Task<CustomerRewardResponseDto?> UseRewardAsync(int id)
    {
        var reward = await _customerRewardRepo.GetCustomerRewardByIdAsync(id);
        if (reward == null)
            return null;

        // only claimed rewards can be used
        if (reward.Status != "Claimed")
            throw new Exception("Reward must be in 'Claimed' status before it can be used.");

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

    // delete customer reward and add back points to customer
    public async Task DeleteCustomerRewardAsync(int id)
    {
        var reward = await _customerRewardRepo.GetCustomerRewardByIdAsync(id);
        if (reward == null)
            throw new Exception("Customer reward not found.");
        if (reward.Status != "Pending")
        {
            throw new Exception("Only pending rewards can be deleted.");
        }
        // add back points to customer
        var customer = await _customerRepo.GetCustomerByIdAsync(reward.CustomerId);
        if (customer != null)
        {
            customer.Points += reward.Reward.PointCost;
            await _customerRepo.UpdateCustomerAsync(customer);
            // update rank 
            await _customerRepo.UpdateCustomerRankAsync(customer.CustomerId);
        }

        // increase reward quantity available
        var rewardItem = await _rewardRepo.GetRewardByIdAsync(reward.RewardId);
        if (rewardItem != null)
        {
            rewardItem.QuantityAvailable += 1;
            await _rewardRepo.UpdateRewardAsync(rewardItem);
        }

        // delete reward
        await _customerRewardRepo.DeleteCustomerRewardAsync(id);
    }

    // customer redeems a reward: 
    // - Check: `Customers.points >= Rewards.point_cost`  
    // - `Rewards.quantity_available > 0`  
    // - Thực hiện trong **transaction** (ở repository):  
    // ① trừ điểm khách hàng  
    // ② giảm `quantity_available`  
    // ③ thêm bản ghi `CustomerRewards`  
    // ④ thêm bản ghi `PointTransactions` (transaction_type='Redeem')
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

        // Step 5: Call repository to handle transaction
        var customerReward = await _customerRewardRepo.RedeemRewardAsync(customerId, rewardId, invoiceId, reward.PointCost);

        // update rank after redeeming
        await _customerRepo.UpdateCustomerRankAsync(customerId);

        // Step 6: Return response DTO
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
}
