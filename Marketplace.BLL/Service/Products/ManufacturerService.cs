using Marketplace.Application.Marketplace.DAL.Contracts;
using Marketplace.BLL.Contracts.Products;
using Marketplace.Domain.Products.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Service.Products
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ManufacturerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Manufacturer> CreateManufacturerAsync(string name)
        {
            var repoManufacturer = _unitOfWork.GetRepository<Manufacturer>();
            var findNameManufacturer = await repoManufacturer.AsQueryable().FirstOrDefaultAsync(m => m.Name == name);
            if (findNameManufacturer != null)
            {
                throw new InvalidOperationException($"Manufacturer '{name}' already exists.");
            }
            var newManufacturer = new Manufacturer { Name =  name };
            repoManufacturer.Create(newManufacturer);
            await _unitOfWork.SaveChangesAsync();
            return newManufacturer;
        }

        public async Task<(bool success, string message)> DeleteManufacturerAsync(int id)
        {
            var manufacturerRepo = _unitOfWork.GetRepository<Manufacturer>();
            var manufacturer = await manufacturerRepo.AsQueryable().FirstOrDefaultAsync(m => m.Identifier == id);

            if (manufacturer == null)
            {
                return (false, $"Manufacturer with ID {id} not found.");
            }


            manufacturerRepo.Delete(manufacturer);
            await _unitOfWork.SaveChangesAsync();

            return (true, $"Manufacturer with ID {id} has been successfully deleted.");
        }

        public async Task<IEnumerable<Manufacturer>> GetAllManufacturersAsync()
        {
            var allManufacturer = await _unitOfWork.GetRepository<Manufacturer>().AsQueryable().ToListAsync();
            return allManufacturer;
        }

        public Task<Manufacturer?> GetManufacturerByIdAsync(int id)
        {
            var findManufacturer = _unitOfWork.GetRepository<Manufacturer>().AsQueryable().FirstOrDefaultAsync(m => m.Identifier == id);
            if (findManufacturer == null)
            {
                throw new KeyNotFoundException($"Manufacturer with ID {id} not found.");
            }
            return findManufacturer;
        }

        public async Task<Manufacturer?> UpdateManufacturerAsync(int id, string name)
        {
            var repoManufacturer = _unitOfWork.GetRepository<Manufacturer>();
            var findManufacturer = repoManufacturer.AsQueryable().FirstOrDefaultAsync(m => m.Identifier == id).Result;

            if (findManufacturer == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            findManufacturer.Name = name;
            repoManufacturer.Update(findManufacturer);
            await _unitOfWork.SaveChangesAsync();
            return findManufacturer;

        }

        public async Task<Manufacturer?> UpdateManufacturerDetailsAsync(int id, string? country, string? website)
        {
            var repoManufacturer = _unitOfWork.GetRepository<Manufacturer>();
            var manufacturer = await repoManufacturer.AsQueryable().FirstOrDefaultAsync(m => m.Identifier == id);

            if (manufacturer == null)
            {
                throw new KeyNotFoundException($"Manufacturer with ID {id} not found.");
            }

           
            if (!string.IsNullOrWhiteSpace(country))
            {
                manufacturer.Country = country;
            }

            if (!string.IsNullOrWhiteSpace(website))
            {
                manufacturer.Website = website;
            }

            repoManufacturer.Update(manufacturer);
            await _unitOfWork.SaveChangesAsync();

            return manufacturer;
        }
    }
}
