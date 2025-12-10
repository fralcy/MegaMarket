namespace MegaMarket.API.DTOs.Reports
{
    public class TopCustomerDto
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int Points { get; set; }
        public string Rank { get; set; } = null!;
    }
}
