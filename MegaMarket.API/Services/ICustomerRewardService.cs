using MegaMarket.API.DTOs.CustomerRewards;

public interface ICustomerRewardService
{
    // get all customer rewards with optional filtering by status and customerId
    Task<IEnumerable<CustomerRewardResponseDto>> GetAllAsync(string? status, int? customerId);

    // get customer rewards by customer id
    Task<IEnumerable<CustomerRewardResponseDto>> GetByCustomerIdAsync(int customerId);

    // use a customer reward by id and update its status to "Used"
    Task<CustomerRewardResponseDto?> UseRewardAsync(int id);

    // delete a customer reward by id and add back points to the customer
    Task DeleteCustomerRewardAsync(int id);
}
