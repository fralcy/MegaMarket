using MegaMarket.Data.Data;
using MegaMarket.Data.Models;
using Microsoft.EntityFrameworkCore;
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
        public async Task<List<Invoice>> GetAllInvoices()
        {
            var listInvoices = new List<Invoice>();
            try
            {
                listInvoices = await _context.Invoices
                    .Include(i => i.User)
                    .Include(i => i.InvoiceDetails)
                        .ThenInclude(id => id.Product)
                    .ToListAsync();
                return listInvoices;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving invoices: " + ex.Message);
            }
        }
        public async Task<Invoice> SaveInvoice(Invoice i)
        {
            try
            {
                _context.Invoices.Add(i);
                await _context.SaveChangesAsync();

                var insertedInvoice = await _context.Invoices
                    .Include(inv => inv.User)
                    .Include(inv => inv.InvoiceDetails)
                        .ThenInclude(id => id.Product)
                    .FirstOrDefaultAsync(inv => inv.InvoiceId == i.InvoiceId);
                return insertedInvoice;
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving invoice: " + ex.Message);
            }
        }
    }
}
