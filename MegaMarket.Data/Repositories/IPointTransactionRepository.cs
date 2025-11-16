using MegaMarket.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMarket.Data.Repositories
{
    public interface IPointTransactionRepository
    {
        // get all pointtracsaction and filter
        Task<IEnumerable<PointTransaction>> GetPointTransactionsAsync(DateTime? fromDate, DateTime? toDate, string? type);

        // get point transaction by id 
        Task<PointTransaction?> GetPointTransactionByIdAsync(int id);

        // get history point transaction of customer by id 
        Task<IEnumerable<PointTransaction>> GetPointHistoryByCustomerIdAsync(int customerId);

        // add and subtract point
        Task<PointTransaction> AddTransactionAsync(PointTransaction transaction);
    }
}
