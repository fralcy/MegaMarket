using MegaMarket.API.DTOs.Customers;
using MegaMarket.API.DTOs.Rewards;
using MegaMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MegaMarket.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RewardsController : ControllerBase
    {
        private readonly IRewardService _rewardService;
        public RewardsController(IRewardService rewardService)
        {
            _rewardService = rewardService;
        }

        //api/rewards : get all rewards
        [HttpGet]
        public async Task<IActionResult> GetAllRewards()
        {
            var rewards = await _rewardService.GetAllRewardsAsync();
            return Ok(rewards);
        }

        //api/rewards/{id} : get reward by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRewardById(int id)
        {
            // get a reward 
            var reward = await _rewardService.GetRewarByIddAsync(id);

            if (reward == null)
            {
                return NotFound();
            }
            return Ok(reward);
        }

        //api/rewards : create a reward
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateReward([FromBody] CreateRewardRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // create a reward
            var response = await _rewardService.CreateRewardAsync(requestDto);
            if (response == null)
            {
                return BadRequest("Can't create reward.");
            }
            return CreatedAtAction(nameof(GetRewardById), new { id = response.RewardId }, response);
        }

        //api/rewards/{id} : update reward
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReward(int id, [FromBody] UpdateRewardRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _rewardService.UpdateRewardAsync(id, requestDto);
            if (response == null)
            {
                return BadRequest("Can't update reward.");
            }
            return Ok(response);
        }

        // api/rewards/{id} : delte reward (is_active = false)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReward(int id)
        {
            var result = await _rewardService.DeleteRewardAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
