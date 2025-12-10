using MegaMarket.API.DTOs.Customers;
using MegaMarket.API.DTOs.Rewards;
using MegaMarket.Data.Models;
using MegaMarket.Data.Repositories;

namespace MegaMarket.API.Services
{
    public class RewardService : IRewardService
    {
        private readonly IRewardRepository _rewardRepository;
        public RewardService(IRewardRepository rewardRepository)
        {
            _rewardRepository = rewardRepository;
        }

        // get all rewards are active
        public async Task<IEnumerable<RewardResponseDto>> GetAllRewardsAsync()
        {
            // get all rewards from db
            var rewards = await _rewardRepository.GetAllRewardsAsync();
            // convert sang dto
            var response = rewards.Select(c => new RewardResponseDto
            {
                RewardId = c.RewardId,
                Name = c.Name,
                Description = c.Description,
                RewardType = c.RewardType,
                PointCost = c.PointCost,
                QuantityAvailable = c.QuantityAvailable,
                Value = c.Value,
                IsActive = c.IsActive,
            });

            return response;
        }



        //get a reward by id
        public async Task<RewardResponseDto?> GetRewarByIddAsync(int id)
        {
            // get a reward
            var reward = await _rewardRepository.GetRewardByIdAsync(id);
            if (reward == null)
            {
                return null;
            }
            // convert to dto
            var response = new RewardResponseDto
            {
                RewardId = reward.RewardId,
                Name = reward.Name,
                Description = reward.Description,
                RewardType = reward.RewardType,
                PointCost = reward.PointCost,
                QuantityAvailable = reward.QuantityAvailable,
                Value = reward.Value,
                IsActive = reward.IsActive,
            };
            return response;
        }


        // create a reward
        public async Task<RewardResponseDto?> CreateRewardAsync(CreateRewardRequestDto reward_dto)
        {
            try
            {
                // create a reward
                var reward = new Reward
                {
                    Name = reward_dto.Name!,
                    Description = reward_dto.Description,
                    PointCost = reward_dto.PointCost,
                    Value = reward_dto.Value,
                    QuantityAvailable = reward_dto .QuantityAvailable,
                    RewardType = reward_dto.RewardType!,
                    IsActive = true
                };

                // save in db by repository
                var createdReward = await _rewardRepository.CreateRewardAsync(reward);

                // convert to response dto 
                var response = new RewardResponseDto
                {
                    RewardId = reward.RewardId,
                    Name = reward.Name,
                    Description = reward.Description,
                    RewardType = reward.RewardType,
                    PointCost = reward.PointCost,
                    QuantityAvailable = reward.QuantityAvailable,
                    Value = reward.Value,
                    IsActive = reward.IsActive,
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when adding reward in service: {ex.Message}");
            }
        }


        // update customer 
        public async Task<RewardResponseDto?> UpdateRewardAsync(int rewardId, UpdateRewardRequestDto updateRewardRequestDto)
        {
            try
            {
                // get reward exist
                var reward_exist = await _rewardRepository.GetRewardByIdAsync(rewardId);
                if (reward_exist == null)
                {
                    return null;
                }

                //create reward
                var reward = new Reward
                {
                    RewardId = rewardId,
                    Name = updateRewardRequestDto.Name!,
                    Description = updateRewardRequestDto.Description,
                    PointCost = updateRewardRequestDto.PointCost,
                    Value = updateRewardRequestDto.Value,
                    RewardType = updateRewardRequestDto.RewardType!,
                    QuantityAvailable = updateRewardRequestDto.QuantityAvailable,
                    IsActive = true
                };

                // save in db by repo
                var updatedReward = await _rewardRepository.UpdateRewardAsync(reward);
                if (updatedReward == null)
                {
                    return null;
                }

                // convert to dto
                var response = new RewardResponseDto
                {
                    RewardId = reward.RewardId,
                    Name = reward.Name,
                    Description = reward.Description,
                    RewardType = reward.RewardType,
                    PointCost = reward.PointCost,
                    QuantityAvailable = reward.QuantityAvailable,
                    Value = reward.Value,
                    IsActive = reward.IsActive,
                };
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when updating reward in service: {ex.Message}");
            }
        }


        // delete reward 
        public async Task<bool> DeleteRewardAsync(int id)
        {
            return await _rewardRepository.DeleteRewardAsync(id);
        }
    }
}
