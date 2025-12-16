using MegaMarket.API.DTOs.Products;
using MegaMarket.Data.Models;

namespace MegaMarket.API.DTOs.Promotion
{
    public class PromotionProductResDto
    {
        public int PromotionId { get; set; }
        public int ProductId { get; set; }
        public PromotionResDto Promotion { get; set; } = null!;
        public ProductDto Product { get; set; } = null!;
    }
}
