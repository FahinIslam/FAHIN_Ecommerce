using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FAHIN_Ecommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Layout"] = "_AdminLayout";
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult ManageProducts()
        {
            return View();
        }
    }
}
