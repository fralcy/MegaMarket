using MegaMarket.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MegaMarket.Data.Repositories
{
    public interface IRewardRepository
    {
        // get all rewards are active
        Task<IEnumerable<Reward>> GetAllRewardsAsync();

        // get a rewards 
        Task<Reward?> GetRewardByIdAsync(int rewardId);

        // create reward
        Task<Reward> CreateRewardAsync(Reward reward);

        // update information of the reward
        Task<Reward?> UpdateRewardAsync(Reward reward);

        // delete soft reward ( is_active = 0 )
        Task<bool> DeleteRewardAsync(int rewardId);
    }
}
