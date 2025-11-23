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
}
