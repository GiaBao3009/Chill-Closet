using Chill_Closet.Models;
namespace Chill_Closet.Repository
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<IEnumerable<Order>> GetAllOrdersAsync();

        Task<Order> GetOrderByIdAsync(int orderId);
        Task UpdateOrderAsync(Order order);

        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
    }
}