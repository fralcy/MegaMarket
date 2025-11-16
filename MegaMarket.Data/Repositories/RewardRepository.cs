using MegaMarket.Data.Data;
using MegaMarket.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMarket.Data.Repositories
{
    public class RewardRepository : IRewardRepository
    {
        private readonly MegaMarketDbContext _context;
        public RewardRepository(MegaMarketDbContext context)
        {
            _context = context;
        }

        // get all rewards are active
        public async Task<IEnumerable<Reward>> GetAllRewardsAsync()
        {
            return await _context.Rewards.AsQueryable().Where(rw => rw.IsActive == true).ToListAsync();
        }

        // get a reward by id
        public async Task<Reward?> GetRewardByIdAsync(int rewardId)
        {
            var reward = await _context.Rewards.FindAsync(rewardId);
            if (reward == null)
            {
                return null;
            }
            return reward;
        }

        // create a reward 
        public async Task<Reward> CreateRewardAsync(Reward reward)
        {
            try
            {
                // add reward in db
                _context.Add(reward);
                await _context.SaveChangesAsync();
                return reward;
            }
            catch (Exception ex)
            {
                // log error 
                throw new Exception($"Error when adding reward in repository: {ex.Message}");
            }
        }

        // update reward
        public async Task<Reward?> UpdateRewardAsync(Reward reward)
        {
            try
            {
                // get reward in db
                var reward_exist = await _context.Rewards.FindAsync(reward.RewardId);
                if (reward_exist == null)
                {
                    return null;
                }

                // save changes in customer
                reward_exist.Name = reward.Name;
                reward_exist.Description = reward.Description;
                reward_exist.RewardType = reward.RewardType;
                reward_exist.PointCost = reward.PointCost;
                reward_exist.Value = reward.Value;
                reward_exist.QuantityAvailable = reward.QuantityAvailable;

                // save in db
                await _context.SaveChangesAsync();
                return reward_exist;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when updating reward in repository: {ex.Message}");
            }
        }

        // delete soft reward ( is_active = 0 )
        public async Task<bool> DeleteRewardAsync(int rewardId)
        {
            var reward = await _context.Rewards.FindAsync(rewardId);
            if(reward == null)
            {
                return false;
            }
            reward.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
