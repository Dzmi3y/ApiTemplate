using Database.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace Database.Entities
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User>? Users { get; set; }
        public DbSet<RefreshToken>? RefreshTokens { get; set; }

    }
}
