using FAHIN_Ecommerce.Data.Entity;

namespace FAHIN_Ecommerce.Services.Interfaces
{
    public interface IOrderService
    {
        Task<int> PlaceOrderAsync(string userId, int productId, int quantity, string? ipAddress, string? userAgent);
        Task<Order?> GetOrderDetailsAsync(int orderId);
    }
}
