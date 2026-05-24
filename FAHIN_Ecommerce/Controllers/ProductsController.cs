using FAHIN_Ecommerce.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FAHIN_Ecommerce.Controllers
{
    public class ProductsController : Controller
    {
        private readonly dbContext _context;

        public ProductsController(dbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var products = categoryId.HasValue 
                ? await _context.Products.Where(p => p.categoryId == categoryId).ToListAsync()
                : await _context.Products.ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
