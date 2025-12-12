using MegaMarket.Data.DataAccess;
using MegaMarket.Data.Models;
using MegaMarket.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMarket.Data.Repositories.Implementations
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly InvoiceDAO _invoiceDAO;
        public InvoiceRepository(InvoiceDAO invoiceDAO)
        {
            _invoiceDAO = invoiceDAO;
        }
        public Task<List<Invoice>> GetAllInvoices() => _invoiceDAO.GetAllInvoices();
        public Task<Invoice> SaveInvoice(Invoice i) => _invoiceDAO.SaveInvoice(i);
    }
}
