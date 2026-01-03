using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using TKH.Core.Contexts;
using TKH.Core.Entities.Abstract;
using TKH.Entities;

namespace TKH.DataAccess.Contexts
{
    public class TKHDbContext : DbContext
    {
        private readonly IWorkContext _workContext;

        public DbSet<MarketplaceAccount> MarketplaceAccounts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryAttribute> CategoryAttributes { get; set; }
        public DbSet<AttributeValue> AttributeValues { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductExpense> ProductExpenses { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<FinancialTransaction> FinancialTransactions { get; set; }
        public DbSet<ShipmentTransaction> ShipmentTransactions { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<ClaimItem> ClaimItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Setting> Settings { get; set; }


        public TKHDbContext(DbContextOptions<TKHDbContext> options) : base(options)
        {
            // for Database Context Factory
        }

        public TKHDbContext(DbContextOptions<TKHDbContext> options, IWorkContext workContext) : base(options)
        {
            _workContext = workContext;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TKHDbContext).Assembly);
            ApplyGlobalMarketplaceFilter(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetMarketplaceAccountIds();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            SetMarketplaceAccountIds();
            return base.SaveChanges();
        }

        private void ApplyGlobalMarketplaceFilter(ModelBuilder modelBuilder)
        {
            Expression<Func<IHasMarketplaceAccount, bool>> filterExpression = expression => expression.MarketplaceAccountId == (_workContext.CurrentMarketplaceAccountId ?? 0);

            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IHasMarketplaceAccount).IsAssignableFrom(entityType.ClrType))
                {
                    ParameterExpression parameterExpression = Expression.Parameter(entityType.ClrType);

                    Expression body = ReplacingExpressionVisitor.Replace(
                        filterExpression.Parameters.First(),
                        parameterExpression,
                        filterExpression.Body);

                    LambdaExpression lambdaExpression = Expression.Lambda(body, parameterExpression);
                    entityType.SetQueryFilter(lambdaExpression);
                }
            }
        }

        private void SetMarketplaceAccountIds()
        {
            var entries = ChangeTracker.Entries<IHasMarketplaceAccount>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity.MarketplaceAccountId == 0 && _workContext.CurrentMarketplaceAccountId.HasValue)
                        entry.Entity.MarketplaceAccountId = _workContext.CurrentMarketplaceAccountId.Value;
                }
            }
        }
    }
}
