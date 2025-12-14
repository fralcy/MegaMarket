using MegaMarket.API.DTOs.Promotion;
using MegaMarket.Data.Models;

namespace MegaMarket.API.Services.Interfaces
{
    public interface IPromotionService
    {
        Task<List<PromotionResDto>> GetAllPromotions();
        Task<PromotionResDto> CreatePromotion(PromotionReqDto promotionDto);
        Task DeletePromotion(int promotionId);
    }
}
