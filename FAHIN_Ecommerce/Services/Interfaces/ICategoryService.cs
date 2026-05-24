using FAHIN_Ecommerce.Data.Entity;

namespace FAHIN_Ecommerce.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
    }
}
