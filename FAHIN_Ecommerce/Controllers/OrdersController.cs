using FAHIN_Ecommerce.Context;
using FAHIN_Ecommerce.Data.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FAHIN_Ecommerce.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly dbContext _context;

        public OrdersController(dbContext context)
        {
            _context = context;
        }

        public IActionResult Checkout(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var order = new Order
            {
                userId = userId,
                orderDate = DateTime.UtcNow,
                totalAmount = product.price * quantity,
                status = "Pending"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // Get the order Id

            var orderItem = new OrderItem
            {
                orderId = order.Id,
                productId = productId,
                quantity = quantity,
                unitPrice = product.price
            };
            _context.OrderItems.Add(orderItem);

            var purchaseLog = new PurchaseLog
            {
                userId = userId,
                productId = productId,
                quantity = quantity,
                totalPrice = product.price * quantity,
                ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                userAgent = Request.Headers["User-Agent"].ToString(),
                details = $"Order placed for {product.name} (Qty: {quantity})"
            };

            _context.PurchaseLogs.Add(purchaseLog);
            await _context.SaveChangesAsync();

            return RedirectToAction("Success", new { orderId = order.Id });
        }

        public IActionResult Success(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}
