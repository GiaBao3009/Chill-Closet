// File: Data/ChillClosetContext.cs

using Chill_Closet.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chill_Closet.Data
{
    // Đã đổi tên lớp và constructor
    public class ChillClosetContext : IdentityDbContext<ApplicationUser>
    {
        public ChillClosetContext(DbContextOptions<ChillClosetContext> options)
            : base(options)
        {
        }

        // Thêm các DbSet cho các model mới
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        // Thêm các DbSet khác ở đây (Reviews, Categories,...)
    }
}