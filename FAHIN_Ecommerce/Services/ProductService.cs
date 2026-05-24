using FAHIN_Ecommerce.Context;
using FAHIN_Ecommerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FAHIN_Ecommerce.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<int> CreateProductAsync(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
    }

    public class ProductService : IProductService
    {
        private readonly dbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(dbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCurrentUser() => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Where(p => p.isDelete == 0)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.category)
                .FirstOrDefaultAsync(p => p.Id == id && p.isDelete == 0);
        }

        public async Task<int> CreateProductAsync(Product product)
        {
            product.createdAt = DateTime.UtcNow;
            product.createdBy = GetCurrentUser();
            product.isDelete = 0;
            
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            var existing = await _context.Products.FindAsync(product.Id);
            if (existing == null || existing.isDelete != 0) return false;

            existing.name = product.name;
            existing.description = product.description;
            existing.price = product.price;
            existing.stockQuantity = product.stockQuantity;
            existing.imageUrl = product.imageUrl;
            existing.categoryId = product.categoryId;
            
            existing.updatedAt = DateTime.UtcNow;
            existing.updatedBy = GetCurrentUser();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.isDelete = 1;
            product.updatedAt = DateTime.UtcNow;
            product.updatedBy = GetCurrentUser();

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
