using Marketplace.Domain.Products.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Contracts.Products
{
    public interface ICategoryService : IService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(string categoryName);
        Task<Category?> UpdateCategoryAsync(int id, string categoryName);
        Task<(bool success, string message)> DeleteCategoryAsync(int id);
    }
}
