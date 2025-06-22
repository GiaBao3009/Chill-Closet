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

        // --- THÊM CÁC PHƯƠNG THỨC MỚI VÀO ĐÂY ---

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}