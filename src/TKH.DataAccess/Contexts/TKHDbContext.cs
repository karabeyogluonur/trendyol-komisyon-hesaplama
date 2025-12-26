using Microsoft.EntityFrameworkCore;
using TKH.Entities;

namespace TKH.DataAccess.Contexts
{
    public class TKHDbContext : DbContext
    {
        public DbSet<MarketplaceAccount> MarketplaceAccounts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryAttribute> CategoryAttributes { get; set; }
        public DbSet<AttributeValue> AttributeValues { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductExpense> ProductExpenses { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<ShipmentTransaction> ShipmentTransactions { get; set; }

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
