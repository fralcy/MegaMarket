using MegaMarket.API.DTOs.Imports;
using MegaMarket.Data.Models;

namespace MegaMarket.API.Services.Interfaces;

public interface IImportService
{
    Task<IEnumerable<Import>> GetAllAsync();
    Task<Import?> GetByIdAsync(int id);
    Task<Import> CreateAsync(ImportCreateDto dto);
    Task<Import?> UpdateAsync(ImportUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
