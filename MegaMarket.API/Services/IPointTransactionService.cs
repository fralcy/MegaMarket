using MegaMarket.API.DTOs.Customers;
using MegaMarket.API.DTOs.PointTransaction;
using MegaMarket.API.DTOs.PointTransactions;
using MegaMarket.Data.Models;

namespace MegaMarket.API.Services
{
    public interface IPointTransactionService
    {
        // get all pointtracsactions and filter
        Task<IEnumerable<PointTransactionResponseDto>> GetFilteredPointTransactionsAsync(FilterPointTransactionRequestDto filter);

        // get a point transaction
        Task<PointTransactionResponseDto?> GetPointTransactionAsyncById(int id);

        // get history point transaction of customer by id 
        Task<IEnumerable<HistoryPointResponseDto>> GetPointHistoryAsync(int customerId);

        // add point
        Task<PointTransactionResponseDto?> AddPointAsync(int customerId, AddPointRequestDto dto);

        // subtract point
        Task<PointTransactionResponseDto?> SubtractPointAsync(int customerId, SubtractPointRequestDto dto);
    }
}
