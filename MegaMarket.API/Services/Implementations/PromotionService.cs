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
        public async Task<List<Promotion>> GetAllPromotions()
        {
            var promotions = await _repository.GetAllPromotions();
            return promotions;
        }
        public async Task<Promotion> CreatePromotion(PromotionDto promotionDto)
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

            return createdPromotion;
        }
        public async Task DeletePromotion(int promotionId)
        {
            await _repository.DeletePromotion(promotionId);
        }
    }
}
