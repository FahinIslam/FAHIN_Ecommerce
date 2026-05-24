using FAHIN_Ecommerce.Context;
using FAHIN_Ecommerce.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace FAHIN_Ecommerce.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
    }

    public class CategoryService : ICategoryService
    {
        private readonly dbContext _context;

        public CategoryService(dbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.isDelete == 0)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.isDelete == 0);
        }
    }
}
