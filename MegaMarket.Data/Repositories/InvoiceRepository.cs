using MegaMarket.Data.DataAccess;
using MegaMarket.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMarket.Data.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly InvoiceDAO _invoiceDAO;
        public InvoiceRepository(InvoiceDAO invoiceDAO)
        {
            _invoiceDAO = invoiceDAO;
        }
        public Task SaveInvoice(Invoice i) => _invoiceDAO.SaveInvoice(i);
    }
}
