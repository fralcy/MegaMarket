using MegaMarket.Data.Data;
using MegaMarket.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMarket.Data.DataAccess
{
    public class InvoiceDAO
    {
        private readonly MegaMarketDbContext _context;
        public InvoiceDAO(MegaMarketDbContext context)
        {
            _context = context;
        }
        public async Task<Invoice> SaveInvoice(Invoice i)
        {
            try
            {
                _context.Invoices.Add(i);
                await _context.SaveChangesAsync();
                return i;
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving invoice: " + ex.Message);
            }
        }
    }
}
