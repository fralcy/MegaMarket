using MegaMarket.API.DTOs.Customers;
using MegaMarket.API.DTOs.Rewards;

namespace MegaMarket.API.Services
{
    public interface IRewardService
    {
        // get all rewards are actice 
        Task<IEnumerable<RewardResponseDto>> GetAllRewardsAsync();

        // get a reward by id
        Task<RewardResponseDto?> GetRewarByIddAsync(int id);

        // create reward
        Task<RewardResponseDto?> CreateRewardAsync(CreateRewardRequestDto reward_dto);

        // update reward
        Task<RewardResponseDto?> UpdateRewardAsync(int rewardId, UpdateRewardRequestDto updateRewardRequestDto);

        // delete reward ( is-active =  false)
        Task<bool> DeleteRewardAsync(int id);
    }
}
