using Microsoft.EntityFrameworkCore;
using TKH.Entities.Concrete;

namespace TKH.DataAccess.Contexts
{
    public class TKHDbContext : DbContext
    {
        public TKHDbContext(DbContextOptions<TKHDbContext> options) : base(options)
        {
        }

        public DbSet<MarketplaceAccount> MarketplaceAccounts { get; set; }

    }
}
