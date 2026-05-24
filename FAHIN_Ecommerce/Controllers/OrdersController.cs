using FAHIN_Ecommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FAHIN_Ecommerce.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public OrdersController(IOrderService orderService, IProductService productService)
        {
            _orderService = orderService;
            _productService = productService;
        }

        public async Task<IActionResult> Checkout(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(int productId, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();

            try
            {
                var orderId = await _orderService.PlaceOrderAsync(userId, productId, quantity, ipAddress, userAgent);
                return RedirectToAction("Success", new { orderId = orderId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var product = await _productService.GetProductByIdAsync(productId);
                return View("Checkout", product);
            }
        }

        public async Task<IActionResult> Success(int orderId)
        {
            var order = await _orderService.GetOrderDetailsAsync(orderId);
            if (order == null) return NotFound();
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}
