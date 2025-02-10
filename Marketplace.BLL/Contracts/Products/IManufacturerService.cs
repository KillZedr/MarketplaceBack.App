using Marketplace.Domain.Products.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Contracts.Products
{
    public interface IManufacturerService : IService
    {
        Task<IEnumerable<Manufacturer>> GetAllManufacturersAsync();
        Task<Manufacturer?> GetManufacturerByIdAsync(int id);
        Task<Manufacturer> CreateManufacturerAsync(string name);
        Task<Manufacturer?> UpdateManufacturerAsync(int id, string name);
        Task<(bool success, string message)> DeleteManufacturerAsync(int id);
        Task<Manufacturer?> UpdateManufacturerDetailsAsync(int id, string? country, string? website);

    }
}
