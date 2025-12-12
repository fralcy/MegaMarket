using MegaMarket.Data.DataAccess;
using MegaMarket.Data.Models;
using MegaMarket.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMarket.Data.Repositories.Implementations
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly PromotionDAO _promotionDAO;
        public PromotionRepository(PromotionDAO promotionDAO)
        {
            _promotionDAO = promotionDAO;
        }
        public Task<List<Promotion>> GetAllPromotions() => _promotionDAO.GetAllPromotions();
        public Task<Promotion> CreatePromotion(Promotion promotion) => _promotionDAO.CreatePromotion(promotion);
        public Task DeletePromotion(int promotionId) => _promotionDAO.DeletePromotion(promotionId);
    }
}
