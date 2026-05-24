using FAHIN_Ecommerce.Context;
using FAHIN_Ecommerce.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace FAHIN_Ecommerce.Services
{
    public interface IOrderService
    {
        Task<int> PlaceOrderAsync(string userId, int productId, int quantity, string? ipAddress, string? userAgent);
        Task<Order?> GetOrderDetailsAsync(int orderId);
    }

    public class OrderService : IOrderService
    {
        private readonly dbContext _context;

        public OrderService(dbContext context)
        {
            _context = context;
        }

        public async Task<int> PlaceOrderAsync(string userId, int productId, int quantity, string? ipAddress, string? userAgent)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || product.isDelete != 0) throw new Exception("Product not found");

            // Create Order
            var order = new Order
            {
                userId = userId,
                orderDate = DateTime.UtcNow,
                totalAmount = product.price * quantity,
                status = "Pending",
                createdAt = DateTime.UtcNow,
                createdBy = userId,
                isDelete = 0
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create Order Item
            var orderItem = new OrderItem
            {
                orderId = order.Id,
                productId = productId,
                quantity = quantity,
                unitPrice = product.price,
                createdAt = DateTime.UtcNow,
                createdBy = userId,
                isDelete = 0
            };
            _context.OrderItems.Add(orderItem);

            // Create Purchase Log
            var purchaseLog = new PurchaseLog
            {
                userId = userId,
                productId = productId,
                quantity = quantity,
                totalPrice = product.price * quantity,
                ipAddress = ipAddress,
                userAgent = userAgent,
                details = $"Order placed for {product.name} (Qty: {quantity})",
                createdAt = DateTime.UtcNow,
                createdBy = userId,
                isDelete = 0
            };
            _context.PurchaseLogs.Add(purchaseLog);

            await _context.SaveChangesAsync();
            return order.Id;
        }

        public async Task<Order?> GetOrderDetailsAsync(int orderId)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.isDelete == 0);
        }
    }
}
