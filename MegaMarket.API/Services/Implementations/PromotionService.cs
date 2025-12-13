using MegaMarket.API.DTOs.Promotion;
using MegaMarket.API.Services.Interfaces;
using MegaMarket.Data.Models;
using MegaMarket.Data.Repositories.Interfaces;

namespace MegaMarket.API.Services.Implementations
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _repository;
        public PromotionService(IPromotionRepository repository)
        {
            _repository = repository;
        }
        public async Task<List<PromotionResponseDto>> GetAllPromotions()
        {
            var promotions = await _repository.GetAllPromotions();
            var result = promotions.Select(p => new PromotionResponseDto
            {
                PromotionId = p.PromotionId,
                Name = p.Name,
                Description = p.Description,
                DiscountType = p.DiscountType,
                DiscountValue = p.DiscountValue,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Type = p.Type,
            }).ToList();
            return result;
        }
        public async Task<PromotionResponseDto> CreatePromotion(PromotionRequestDto promotionDto)
        {
            var promotion = new Promotion
            {
                Name = promotionDto.Name,
                Description = promotionDto.Description,
                DiscountType = promotionDto.DiscountType,
                DiscountValue = promotionDto.DiscountValue,
                StartDate = promotionDto.StartDate,
                EndDate = promotionDto.EndDate,
                Type = promotionDto.Type,
            };
            var createdPromotion = await _repository.CreatePromotion(promotion);
            var result = new PromotionResponseDto
            {
                PromotionId = createdPromotion.PromotionId,
                Name = createdPromotion.Name,
                Description = createdPromotion.Description,
                DiscountType = createdPromotion.DiscountType,
                DiscountValue = createdPromotion.DiscountValue,
                StartDate = createdPromotion.StartDate,
                EndDate = createdPromotion.EndDate,
                Type = createdPromotion.Type,
            };

            return result;
        }
        public async Task DeletePromotion(int promotionId)
        {
            await _repository.DeletePromotion(promotionId);
        }
    }
}
