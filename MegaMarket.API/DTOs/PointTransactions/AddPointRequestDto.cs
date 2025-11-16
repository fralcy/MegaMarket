namespace MegaMarket.API.DTOs.PointTransaction
{
    public class AddPointRequestDto
    {
        public int Points { get; set; }
        public int? InvoiceId { get; set; } // optional
        public string? Description { get; set; }
    }
}
