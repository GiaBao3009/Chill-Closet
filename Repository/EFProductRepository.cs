using Chill_Closet.Data;
using Chill_Closet.Models;
using Microsoft.EntityFrameworkCore;

namespace Chill_Closet.Repository
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ChillClosetContext _context;
        public EFProductRepository(ChillClosetContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            // Include(p => p.Category) để lấy thông tin cả danh mục
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            
            return await _context.Products
                                 .Include(p => p.Category)
                                 .Include(p => p.Images)
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}