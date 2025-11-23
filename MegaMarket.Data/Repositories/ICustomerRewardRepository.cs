using MegaMarket.Data.Models;
public interface ICustomerRewardRepository
{
    // get all customer rewards with optional filters( status, customerId)
    Task<IEnumerable<CustomerReward>> GetAllAsync(string? status, int? customerId);

    // get customer rewards by customerId
    Task<IEnumerable<CustomerReward>> GetByCustomerIdAsync(int customerId);

    // get customer reward by id
    Task<CustomerReward?> GetCustomerRewardByIdAsync(int id);

    // update status of customer reward
    Task<CustomerReward> UpdateCustomerRewardAsync(CustomerReward reward);

    // delete customer reward by id and add again point for customer
    Task DeleteCustomerRewardAsync(int id);
}
