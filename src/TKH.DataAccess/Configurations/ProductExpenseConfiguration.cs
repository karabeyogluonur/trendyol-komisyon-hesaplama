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

            builder.Property(productExpense => productExpense.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(productExpense => productExpense.VatRate)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(productExpense => productExpense.IsVatIncluded)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(productExpense => productExpense.Type).IsRequired();
            builder.Property(productExpense => productExpense.StartDate).IsRequired();

            builder.HasOne(productExpense => productExpense.Product)
                .WithMany(p => p.ProductExpenses)
                .HasForeignKey(productExpense => productExpense.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(productExpense => new { productExpense.ProductId, productExpense.Type, productExpense.EndDate })
                .HasDatabaseName("IX_ProductExpenses_ActiveLookup");

            builder.HasIndex(productExpense => new { productExpense.ProductId, productExpense.Type, productExpense.StartDate })
               .HasDatabaseName("IX_ProductExpenses_HistoryLookup");
        }
    }
}
