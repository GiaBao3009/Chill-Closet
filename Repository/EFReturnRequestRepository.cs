using Chill_Closet.Data;
using Chill_Closet.Models;
using Microsoft.EntityFrameworkCore;
namespace Chill_Closet.Repository
{
    public class EFReturnRequestRepository : IReturnRequestRepository
    {
        private readonly ChillClosetContext _context;
        public EFReturnRequestRepository(ChillClosetContext context) { _context = context; }

        public async Task AddAsync(ReturnRequest returnRequest)
        {
            _context.ReturnRequests.Add(returnRequest);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<ReturnRequest>> GetAllAsync()
        {
            // Lấy tất cả yêu cầu, bao gồm thông tin đơn hàng, sắp xếp theo ngày mới nhất
            return await _context.ReturnRequests
                                 .Include(r => r.Order)
                                 .OrderByDescending(r => r.RequestDate)
                                 .ToListAsync();
        }

        public async Task<ReturnRequest> GetByIdAsync(int id)
        {
            // Lấy một yêu cầu cụ thể, bao gồm chi tiết đơn hàng và hình ảnh
            return await _context.ReturnRequests
                                 .Include(r => r.Order)
                                 .Include(r => r.Images)
                                 .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task UpdateAsync(ReturnRequest returnRequest)
        {
            _context.ReturnRequests.Update(returnRequest);
            await _context.SaveChangesAsync();
        }
    }
}