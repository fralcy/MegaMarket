namespace MegaMarket.API.DTOs.PointTransaction
{
    public class SubtractPointRequestDto
    {
        public int Points { get; set; }  
        public string TransactionType { get; set; } = ""; 
        public string? Description { get; set; }         
    }
}
