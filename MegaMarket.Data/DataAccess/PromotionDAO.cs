using MegaMarket.Data.Data;
using MegaMarket.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMarket.Data.DataAccess
{
    public class PromotionDAO
    {
        private readonly MegaMarketDbContext _context;
        public PromotionDAO(MegaMarketDbContext context)
        {
            _context = context;
        }
        // Data Access Object methods for Promotion entity
        public async Task<List<Promotion>> GetAllPromotions()
        {
            var listPromotions = new List<Promotion>();
            try
            {
                listPromotions = await _context.Promotions.ToListAsync();
                return listPromotions;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving promotions: " + ex.Message);
            }
        }
        public async Task<Promotion> CreatePromotion(Promotion promotion)
        {
            try
            {
                _context.Promotions.Add(promotion);
                await _context.SaveChangesAsync();
                return promotion;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating promotion: " + ex.Message);
            }
        }
        public async Task<Promotion> UpdatePromotion(Promotion promotion)
        {
            try
            {
                _context.Promotions.Update(promotion);
                await _context.SaveChangesAsync();
                return promotion;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating promotion: " + ex.Message);
            }
        }
        public async Task DeletePromotion(int promotionId)
        {
            try
            {
                var promotion = await _context.Promotions.FindAsync(promotionId);
                if (promotion != null)
                {
                    _context.Promotions.Remove(promotion);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Promotion not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting promotion: " + ex.Message);
            }
        }
    }
}
