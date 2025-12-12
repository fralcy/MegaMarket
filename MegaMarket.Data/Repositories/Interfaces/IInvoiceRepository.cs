using MegaMarket.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMarket.Data.Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice> SaveInvoice(Invoice i);
        Task<List<Invoice>> GetAllInvoices();
    }
}
