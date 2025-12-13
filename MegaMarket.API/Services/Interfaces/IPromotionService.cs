using MegaMarket.API.DTOs.Promotion;
using MegaMarket.Data.Models;

namespace MegaMarket.API.Services.Interfaces
{
    public interface IPromotionService
    {
        Task<List<PromotionResponseDto>> GetAllPromotions();
        Task<PromotionResponseDto> CreatePromotion(PromotionRequestDto promotionDto);
        Task DeletePromotion(int promotionId);
    }
}
