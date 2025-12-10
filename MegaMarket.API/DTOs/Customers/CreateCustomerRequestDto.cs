using System.ComponentModel.DataAnnotations;

namespace MegaMarket.API.DTOs.Customers
{
    public class CreateCustomerRequestDto
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Range(0, int.MaxValue)]
        public int Points { get; set; } = 0;
    }
}
