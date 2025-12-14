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
        // Service methods for Promotion entity
        public async Task<List<PromotionResDto>> GetAllPromotions()
        {
            var promotions = await _repository.GetAllPromotions();
            var result = promotions.Select(p => new PromotionResDto
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
        public async Task<PromotionResDto> CreatePromotion(PromotionReqDto promotionDto)
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
            foreach (var productId in promotionDto.PromotionProducts)
            {
                promotion.PromotionProducts.Add(new PromotionProduct
                {
                    ProductId = productId
                });
            }
            var createdPromotion = await _repository.CreatePromotion(promotion);
            var result = new PromotionResDto
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
        public async Task<PromotionResDto> UpdatePromotion(int promotionId, PromotionReqDto promotionDto)
        {
            var promotion = new Promotion
            {
                PromotionId = promotionId,
                Name = promotionDto.Name,
                Description = promotionDto.Description,
                DiscountType = promotionDto.DiscountType,
                DiscountValue = promotionDto.DiscountValue,
                StartDate = promotionDto.StartDate,
                EndDate = promotionDto.EndDate,
                Type = promotionDto.Type,
            };
            var updatedPromotion = await _repository.UpdatePromotion(promotion);
            var result = new PromotionResDto
            {
                PromotionId = updatedPromotion.PromotionId,
                Name = updatedPromotion.Name,
                Description = updatedPromotion.Description,
                DiscountType = updatedPromotion.DiscountType,
                DiscountValue = updatedPromotion.DiscountValue,
                StartDate = updatedPromotion.StartDate,
                EndDate = updatedPromotion.EndDate,
                Type = updatedPromotion.Type,
            };
            return result;
        }
        public async Task DeletePromotion(int promotionId)
        {
            await _repository.DeletePromotion(promotionId);
        }
    }
}
