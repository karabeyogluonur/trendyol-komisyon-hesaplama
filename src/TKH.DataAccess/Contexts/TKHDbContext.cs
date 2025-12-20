using Microsoft.EntityFrameworkCore;

namespace TKH.DataAccess.Contexts
{
    public class TKHDbContext : DbContext
    {
        public TKHDbContext(DbContextOptions<TKHDbContext> options) : base(options)
        {
        }

    }
}
