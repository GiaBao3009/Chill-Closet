using Chill_Closet.Data;
using Chill_Closet.Models;
using Microsoft.EntityFrameworkCore;

namespace Chill_Closet.Repository
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private readonly ChillClosetContext _context;
        public EFCategoryRepository(ChillClosetContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }
    }
}