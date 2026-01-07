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

            builder.HasKey(productExpense => productExpense.Id);

            builder.Property(productExpense => productExpense.Amount).HasPrecision(18, 2).IsRequired();
            builder.Property(productExpense => productExpense.VatRate).HasPrecision(18, 2).IsRequired();
            builder.Property(productExpense => productExpense.IsVatIncluded).HasDefaultValue(false).IsRequired();
            builder.Property(productExpense => productExpense.Type).IsRequired();
            builder.Property(productExpense => productExpense.GenerationType).IsRequired();
            builder.Property(productExpense => productExpense.StartDate).IsRequired();
            builder.Property(productExpense => productExpense.EndDate).IsRequired(false);

            builder.HasIndex(productExpense => new { productExpense.ProductId, productExpense.Type, productExpense.GenerationType })
                .HasDatabaseName("IX_ProductExpenses_Filter");

            builder.HasIndex(productExpense => productExpense.EndDate)
                .HasDatabaseName("IX_ProductExpenses_ActiveRecords");

        }
    }
}
