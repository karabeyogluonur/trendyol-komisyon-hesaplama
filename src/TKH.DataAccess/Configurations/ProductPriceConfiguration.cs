using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities;

namespace TKH.DataAccess.Configurations
{
    public class ProductPriceConfiguration : IEntityTypeConfiguration<ProductPrice>
    {
        public void Configure(EntityTypeBuilder<ProductPrice> builder)
        {
            builder.ToTable("ProductPrices");

            builder.HasKey(productPrice => productPrice.Id);

            builder.Property(productPrice => productPrice.Amount).HasPrecision(18, 2).IsRequired();

            builder.Property(productPrice => productPrice.IsVatIncluded).HasDefaultValue(true).IsRequired();

            builder.Property(productPrice => productPrice.Type).IsRequired();

            builder.Property(productPrice => productPrice.StartDate).IsRequired();

            builder.Property(productPrice => productPrice.EndDate).IsRequired(false);

            builder.HasOne(productPrice => productPrice.Product)
                .WithMany(product => product.Prices)
                .HasForeignKey(productPrice => productPrice.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(productPrice => new { productPrice.ProductId, productPrice.Type })
                .HasDatabaseName("IX_ProductPrices_Filter");

            builder.HasIndex(productPrice => productPrice.EndDate)
                .HasDatabaseName("IX_ProductPrices_ActiveRecords");
        }
    }
}
