using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities;

namespace TKH.DataAccess.Configurations
{
    public class ProductExpenseConfiguration : IEntityTypeConfiguration<ProductExpense>
    {
        public void Configure(EntityTypeBuilder<ProductExpense> builder)
        {
            builder.ToTable("ProductExpenses");

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.VatRate)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.IsVatIncluded)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.StartDate).IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany(p => p.ProductExpenses)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.ProductId, x.Type, x.EndDate })
                .HasDatabaseName("IX_ProductExpenses_ActiveLookup");

            builder.HasIndex(x => new { x.ProductId, x.Type, x.StartDate })
               .HasDatabaseName("IX_ProductExpenses_HistoryLookup");
        }
    }
}
