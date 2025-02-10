using Marketplace.Application.Marketplace.DAL.Contracts;
using Marketplace.BLL.Contracts.Products;
using Marketplace.Domain.Products.Entity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Service.Products
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Category> CreateCategoryAsync(string categoryName)
        {
            var categoryRepo =  _unitOfWork.GetRepository<Category>();
            var existingCategory = categoryRepo.AsQueryable().FirstOrDefault(c => c.Name == categoryName);
            if (existingCategory != null) 
            {
                throw new InvalidOperationException($"Category: {categoryName} already exist");
            }
            var newCategory = new Category { Name = categoryName };
            categoryRepo.Create(newCategory);
            await _unitOfWork.SaveChangesAsync();
            return newCategory;
            
        }

        public async Task<(bool success, string message)> DeleteCategoryAsync(int id)
        {
            var repoCategory = _unitOfWork.GetRepository<Category>();
            var deletedCategory = await repoCategory.AsQueryable().FirstOrDefaultAsync(c => c.Identifier == id);
            if (deletedCategory == null)
            {
                return (false, $"Category with ID {id} not found.");
            }
            repoCategory.Delete(deletedCategory);
            await _unitOfWork.SaveChangesAsync();

            return (true, $"Category with ID {id} has been successfully deleted.");
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var allCategory = await _unitOfWork.GetRepository<Category>().AsQueryable().ToListAsync();
            return allCategory;
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            var findCategory = await _unitOfWork.GetRepository<Category>().AsQueryable().FirstOrDefaultAsync(c => c.Identifier == id);
            if (findCategory == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            return findCategory;
        }

        public async Task<Category?> UpdateCategoryAsync(int id, string categoryName)
        {
            var categoryRepo = _unitOfWork.GetRepository<Category>();
            var findCatetegory = categoryRepo.AsQueryable().FirstOrDefaultAsync(c => c.Identifier == id).Result;
            if (findCatetegory == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            findCatetegory.Name = categoryName;
            categoryRepo.Update(findCatetegory);
            await _unitOfWork.SaveChangesAsync();
            return findCatetegory;
        }
    }
}
