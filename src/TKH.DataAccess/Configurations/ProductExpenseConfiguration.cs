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

            builder.Property(productExpense => productExpense.Amount).HasPrecision(18, 2);
            builder.Property(productExpense => productExpense.VatRate).HasPrecision(18, 2);

            builder.HasOne(productExpense => productExpense.Product)
                   .WithMany(product => product.Expenses)
                   .HasForeignKey(productExpense => productExpense.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
