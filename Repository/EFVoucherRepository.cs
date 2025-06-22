using Chill_Closet.Data;
using Chill_Closet.Models;
using Microsoft.EntityFrameworkCore;

namespace Chill_Closet.Repository
{
    public class EFVoucherRepository : IVoucherRepository // <-- THÊM ": IVoucherRepository" VÀO ĐÂY
    {
        private readonly ChillClosetContext _context;
        public EFVoucherRepository(ChillClosetContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Voucher>> GetAllAsync() => await _context.Vouchers.ToListAsync();

        public async Task<Voucher> GetByIdAsync(int id) => await _context.Vouchers.FindAsync(id);

        public async Task<Voucher> GetByCodeAsync(string code) => await _context.Vouchers.FirstOrDefaultAsync(v => v.Code == code && v.ExpiryDate > DateTime.Now && v.Quantity > 0);

        public async Task AddAsync(Voucher voucher)
        {
            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Voucher voucher)
        {
            _context.Vouchers.Update(voucher);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher != null)
            {
                _context.Vouchers.Remove(voucher);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Voucher>> GetAvailableForUserAsync(string userId)
        {
            return await _context.Vouchers
                .Where(v => (v.ApplicationUserId == userId || v.ApplicationUserId == null) && v.ExpiryDate > DateTime.Now && v.Quantity > 0)
                .OrderByDescending(v => v.Id)
                .ToListAsync();
        }
    }
}