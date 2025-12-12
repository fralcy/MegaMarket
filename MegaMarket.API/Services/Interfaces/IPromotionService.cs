using MegaMarket.API.DTOs.Promotion;
using MegaMarket.Data.Models;

namespace MegaMarket.API.Services.Interfaces
{
    public interface IPromotionService
    {
        Task<List<Promotion>> GetAllPromotions();
        Task<Promotion> CreatePromotion(PromotionDto promotionDto);
        Task DeletePromotion(int promotionId);
    }
}
