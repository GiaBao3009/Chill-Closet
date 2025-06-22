using Chill_Closet.Data;
using Chill_Closet.Models;
using Microsoft.EntityFrameworkCore;

namespace Chill_Closet.Repository
{
    public class EFOrderRepository : IOrderRepository
    {
        private readonly ChillClosetContext _context;
        public EFOrderRepository(ChillClosetContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                                 .Include(o => o.ApplicationUser) // Lấy thông tin người dùng
                                 .OrderByDescending(o => o.OrderDate) // Sắp xếp theo ngày mới nhất
                                 .ToListAsync();
        }
        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                                 .Include(o => o.ApplicationUser)
                                 .Include(o => o.OrderItems)
                                 .ThenInclude(oi => oi.Product)
                                 .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                                 .Where(o => o.ApplicationUserId == userId)
                                 .OrderByDescending(o => o.OrderDate)
                                 .ToListAsync();
        }
    }
}