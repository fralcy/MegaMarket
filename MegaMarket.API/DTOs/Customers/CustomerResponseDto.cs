using System.Runtime.CompilerServices;

namespace MegaMarket.API.DTOs.Customers
{
    public class CustomerResponseDto
    {
        public int CustomerId { get; set; }
        public string? FullName { get; set; }
        public string? Phone {  get; set; }
        public string? Email { get; set; }
        public int Points { get; set; }
        public string? Rank { get; set; }
    }
}
