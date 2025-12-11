namespace MegaMarket.API.DTOs.Customers
{
    public class FilterCustomerRequestDto
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Rank { get; set; } // Silver / Gold / Platinum
    }
}
