using MegaMarket.API.DTOs.Customers;
using MegaMarket.API.DTOs.PointTransaction;
using MegaMarket.API.DTOs.PointTransactions;
using MegaMarket.Data.Models;
using MegaMarket.Data.Repositories;

namespace MegaMarket.API.Services
{
    public class PointTransactionService : IPointTransactionService
    {
        private readonly IPointTransactionRepository _pointTransactionRepository;
        private readonly ICustomerRepository _customerRepository;
        public PointTransactionService(IPointTransactionRepository pointTransactionRepository, ICustomerRepository customerRepository)
        {
            _pointTransactionRepository = pointTransactionRepository;
            _customerRepository = customerRepository;
        }


        // get a point transaction
        public async Task<PointTransactionResponseDto?> GetPointTransactionAsyncById(int id)
        {
            // get point transaction by id
            var point_transaction = await _pointTransactionRepository.GetPointTransactionByIdAsync(id);
            // check exist
            if (point_transaction == null)
            {
                return null;
            }
            // return for client
            return new PointTransactionResponseDto
            {
                TransactionId = point_transaction.TransactionId,
                CustomerId = point_transaction.CustomerId,
                InvoiceId = point_transaction.InvoiceId,
                PointChange = point_transaction.PointChange,
                TransactionType = point_transaction.TransactionType,
                CreatedAt = point_transaction.CreatedAt,
                Description = point_transaction.Description,
                CustomerName = point_transaction.Customer?.FullName
            };
        }


        // filter poit transactions
        public async Task<IEnumerable<PointTransactionResponseDto>> GetFilteredPointTransactionsAsync(FilterPointTransactionRequestDto filter)
        {
            var transactions = await _pointTransactionRepository.GetPointTransactionsAsync(
                filter.FromDate,
                filter.ToDate,
                filter.TransactionType
            );

            return transactions.Select(pt => new PointTransactionResponseDto
            {
                TransactionId = pt.TransactionId,
                CustomerId = pt.CustomerId,
                InvoiceId = pt.InvoiceId,
                PointChange = pt.PointChange,
                TransactionType = pt.TransactionType,
                CreatedAt = pt.CreatedAt,
                Description = pt.Description,
                CustomerName = pt.Customer?.FullName
            });
        }

        // get history point transaction of customer by id 
        public async Task<IEnumerable<HistoryPointResponseDto>> GetPointHistoryAsync(int customerId)
        {
            var list = await _pointTransactionRepository.GetPointHistoryByCustomerIdAsync(customerId);

            return list.Select(p => new HistoryPointResponseDto
            {
                TransactionId = p.TransactionId,
                PointChange = p.PointChange,
                TransactionType = p.TransactionType,
                CreatedAt = p.CreatedAt,
                Description = p.Description,
                InvoiceId = p.InvoiceId,
            });
        }

        // add point for customer
        public async Task<PointTransactionResponseDto?> AddPointAsync(int customerId, AddPointRequestDto dto)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
            if (customer == null)
                return null;

            int newPoints = customer.Points + dto.Points;
            await _customerRepository.UpdateCustomerPointsAsync(customerId, newPoints);

            var pt = new PointTransaction
            {
                CustomerId = customerId,
                InvoiceId = dto.InvoiceId,
                PointChange = dto.Points,
                TransactionType = "Earn",
                CreatedAt = DateTime.Now,
                Description = dto.Description
                    ?? (dto.InvoiceId != null ? $"Earned from invoice #{dto.InvoiceId}" : "Earned manually"),
                Customer = customer  // include to display customer name 
            };

            var created = await _pointTransactionRepository.AddTransactionAsync(pt);

            return new PointTransactionResponseDto
            {
                TransactionId = created.TransactionId,
                CustomerId = created.CustomerId,
                CustomerName = customer.FullName,        // map with customer 
                InvoiceId = created.InvoiceId,
                PointChange = created.PointChange,
                TransactionType = created.TransactionType,
                CreatedAt = created.CreatedAt,
                Description = created.Description
            };
        }



    }
}
