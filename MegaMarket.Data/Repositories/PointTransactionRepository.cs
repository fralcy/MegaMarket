using MegaMarket.Data.Data;
using MegaMarket.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMarket.Data.Repositories
{
    public class PointTransactionRepository : IPointTransactionRepository
    {
        private readonly MegaMarketDbContext _context;
        public PointTransactionRepository(MegaMarketDbContext context)
        {
            _context = context;
        }

        // get all point transactions and filter 
        public async Task<IEnumerable<PointTransaction>> GetPointTransactionsAsync(DateTime? fromDate, DateTime? toDate, string? type)
        {
            var query = _context.PointTransactions
                                .Include(pt => pt.Customer)
                                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(pt => pt.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(pt => pt.CreatedAt <= toDate.Value);

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(pt => pt.TransactionType == type);

            return await query
                .OrderByDescending(pt => pt.CreatedAt)
                .ToListAsync();
        }

        // get point transaction by id
        public async Task<PointTransaction?> GetPointTransactionByIdAsync(int id)
        {
            // find point transaction in db
            var point_transaction = await _context.PointTransactions.FindAsync(id);
            if (point_transaction != null)
            {
                return point_transaction;
            }
            return null;
        }


        // get history point transaction of customer by id 
        public async Task<IEnumerable<PointTransaction>> GetPointHistoryByCustomerIdAsync(int customerId)
        {
            return await _context.PointTransactions
                .Where(p => p.CustomerId == customerId)
                .Include(pt => pt.Customer)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // add and subtract point 
        public async Task<PointTransaction> AddTransactionAsync(PointTransaction transaction)
        {
            _context.PointTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }
    }
}
