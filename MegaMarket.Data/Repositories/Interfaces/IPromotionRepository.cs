using MegaMarket.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMarket.Data.Repositories.Interfaces
{
    public interface IPromotionRepository
    {
        Task<List<Promotion>> GetAllPromotions();
        Task<Promotion> CreatePromotion(Promotion promotion);
        Task<Promotion> UpdatePromotion(Promotion promotion);
        Task DeletePromotion(int promotionId);
    }
}
