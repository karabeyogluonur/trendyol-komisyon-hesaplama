using Microsoft.EntityFrameworkCore;
using TKH.Entities;

namespace TKH.DataAccess.Contexts
{
    public class TKHDbContext : DbContext
    {
        public DbSet<MarketplaceAccount> MarketplaceAccounts { get; set; }
        public DbSet<Product> Products { get; set; }

        public TKHDbContext(DbContextOptions<TKHDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TKHDbContext).Assembly);
        }

    }
}
