using Chill_Closet.Models;

namespace Chill_Closet.Repository
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
    }
}