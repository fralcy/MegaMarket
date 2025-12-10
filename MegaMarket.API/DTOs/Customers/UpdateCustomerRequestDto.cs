using System.ComponentModel.DataAnnotations;

namespace MegaMarket.API.DTOs.Customers
{
    public class UpdateCustomerRequestDto
    {
        public string? FullName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int Points { get; set; }
        public string? Rank { get; set; }
    }
}
