using MegaMarket.Data;
using MegaMarket.Data.Data;
using MegaMarket.Data.Models;
using Microsoft.EntityFrameworkCore;

public class CustomerRewardRepository : ICustomerRewardRepository
{
    private readonly MegaMarketDbContext _context;

    public CustomerRewardRepository(MegaMarketDbContext context)
    {
        _context = context;
    }

    // get all customer rewards with optional filtering by status and customerId
    public async Task<IEnumerable<CustomerReward>> GetAllAsync(string? status, int? customerId)
    {
        var query = _context.CustomerRewards
            .Include(cr => cr.Customer)
            .Include(cr => cr.Reward)
            .Include(cr => cr.Invoice)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(cr => cr.Status == status);

        if (customerId.HasValue)
            query = query.Where(cr => cr.CustomerId == customerId.Value);

        return await query
            .OrderByDescending(x => x.RedeemedAt)
            .ToListAsync();
    }

    // get customer rewards by customer id
    public async Task<IEnumerable<CustomerReward>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.CustomerRewards
            .Include(cr => cr.Customer)
            .Include(cr => cr.Reward)
            .Include(cr => cr.Invoice)
            .Where(cr => cr.CustomerId == customerId)
            .OrderByDescending(cr => cr.RedeemedAt)
            .ToListAsync();
    }

    // get customer reward by id
    public async Task<CustomerReward?> GetCustomerRewardByIdAsync(int id)
    {
        return await _context.CustomerRewards
            .Include(cr => cr.Customer)
            .Include(cr => cr.Reward)
            .Include(cr => cr.Invoice)
            .FirstOrDefaultAsync(cr => cr.RedemptionId == id);
    }


    // update status of customer reward
    public async Task<CustomerReward> UpdateCustomerRewardAsync(CustomerReward reward)
    {
        _context.CustomerRewards.Update(reward);
        await _context.SaveChangesAsync();
        return reward;
    }

    // delete customer reward by id and add again point for customer
    public async Task DeleteCustomerRewardAsync(int id)
    {
        var reward = await _context.CustomerRewards.FindAsync(id);
        if (reward != null)
        {
            _context.CustomerRewards.Remove(reward);
            await _context.SaveChangesAsync();
        }
    }

    // redeem reward with transaction: deduct points, reduce reward quantity, create CustomerReward and PointTransaction
    public async Task<CustomerReward> RedeemRewardAsync(int customerId, int rewardId, int? invoiceId, int pointCost)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                // ① Trừ điểm khách hàng
                var customer = await _context.Customers.FindAsync(customerId);
                if (customer == null)
                    throw new Exception("Customer not found.");
                
                customer.Points -= pointCost;
                _context.Customers.Update(customer);

                // ② Giảm quantity_available của reward
                var reward = await _context.Rewards.FindAsync(rewardId);
                if (reward == null)
                    throw new Exception("Reward not found.");
                
                reward.QuantityAvailable -= 1;
                _context.Rewards.Update(reward);

                // ③ Thêm bản ghi CustomerRewards với status = "Pending"
                var customerReward = new CustomerReward
                {
                    CustomerId = customerId,
                    RewardId = rewardId,
                    InvoiceId = invoiceId,
                    RedeemedAt = DateTime.Now,
                    Status = "Pending"
                };
                _context.CustomerRewards.Add(customerReward);

                // ④ Thêm bản ghi PointTransaction với transaction_type = "Redeem"
                var pointTransaction = new PointTransaction
                {
                    CustomerId = customerId,
                    InvoiceId = invoiceId,
                    PointChange = -pointCost,
                    TransactionType = "Redeem",
                    CreatedAt = DateTime.Now,
                    Description = $"Redeemed reward: {reward.Name}"
                };
                _context.PointTransactions.Add(pointTransaction);

                // Save all changes
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return customerReward;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error redeeming reward: {ex.Message}", ex);
            }
        }
    }
}
