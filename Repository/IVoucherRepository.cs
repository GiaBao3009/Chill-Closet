using Chill_Closet.Models;
namespace Chill_Closet.Repository
{
    public interface IVoucherRepository
    {
        Task<IEnumerable<Voucher>> GetAllAsync();
        Task<Voucher> GetByIdAsync(int id);
        Task<Voucher> GetByCodeAsync(string code);
        Task AddAsync(Voucher voucher);
        Task UpdateAsync(Voucher voucher);
        Task DeleteAsync(int id);
    }
}