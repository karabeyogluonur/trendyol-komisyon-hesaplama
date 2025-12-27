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
            builder.Property(productPrice => productPrice.Amount).HasPrecision(18, 2);

            builder.HasOne(productPrice => productPrice.Product)
                   .WithMany(product => product.Prices)
                   .HasForeignKey(productPrice => productPrice.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
