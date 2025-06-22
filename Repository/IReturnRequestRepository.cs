using Chill_Closet.Models;
namespace Chill_Closet.Repository
{
    public interface IReturnRequestRepository
    {
        Task AddAsync(ReturnRequest returnRequest);
        Task<IEnumerable<ReturnRequest>> GetAllAsync(); // Thêm mới
        Task<ReturnRequest> GetByIdAsync(int id);      // Thêm mới
        Task UpdateAsync(ReturnRequest returnRequest);  // Thêm mới
    }
}