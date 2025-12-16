using MegaMarket.API.DTOs.Promotion;
using MegaMarket.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MegaMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _service;
        public PromotionController(IPromotionService service)
        {
            _service = service;
        }
        // Controller methods for Promotion entity
        [HttpGet]
        public async Task<ActionResult> GetAllPromotions()
        {
            try
            {
                var promotions = await _service.GetAllPromotions();
                return Ok(promotions);
            }
            catch
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreatePromotion([FromBody] PromotionReqDto promotionDto)
        {
            try
            {
                var createdPromotion = await _service.CreatePromotion(promotionDto);
                return Ok(createdPromotion);
            }
            catch
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{promotionId}")]
        public async Task<ActionResult> UpdatePromotion(int promotionId, [FromBody] PromotionReqDto promotionDto)
        {
            try
            {
                var updatedPromotion = await _service.UpdatePromotion(promotionId, promotionDto);
                return Ok(updatedPromotion);
            }
            catch
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{promotionId}")]
        public async Task<ActionResult> DeletePromotion(int promotionId)
        {
            try
            {
                await _service.DeletePromotion(promotionId);
                return Ok("Promotion deleted successfully");
            }
            catch
            {
                return BadRequest(ModelState);
            }
        }

        [HttpGet("PromotionProduct")]
        public async Task<ActionResult> GetAllPromotionProducts()
        {
            try
            {
                var promotionProducts = await _service.GetAllPromotionProducts();
                return Ok(promotionProducts);
            }
            catch
            {
                return BadRequest(ModelState);
            }
        }
    }
}
