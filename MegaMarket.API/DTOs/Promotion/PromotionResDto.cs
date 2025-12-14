namespace MegaMarket.API.DTOs.Promotion
{
    public class PromotionResDto
    {
        public int PromotionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = "percent"; // percent / fixed
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Type { get; set; } = "invoice"; // invoice / product / promotion
    }
}
