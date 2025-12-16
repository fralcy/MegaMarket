using MegaMarket.Data.Models;

namespace MegaMarket.API.DTOs.Promotion
{
    public class PromotionReqDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = "percent"; // percent / fixed
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(7);
        public string Type { get; set; } = "invoice"; // invoice / product / promotion
        public ICollection<int> PromotionProducts { get; set; } = new List<int>();
    }
}
