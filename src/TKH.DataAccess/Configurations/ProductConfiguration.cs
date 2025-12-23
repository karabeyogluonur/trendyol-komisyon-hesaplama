using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities;

namespace TKH.DataAccess.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.Property(product => product.MarketplaceAccountId)
                .IsRequired();

            builder.Property(product => product.MarketplaceProductId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(product => product.Barcode)
                .HasMaxLength(100);

            builder.Property(product => product.StockCode)
                .HasMaxLength(100);

            builder.Property(product => product.ModelCode)
                .HasMaxLength(100);

            builder.Property(product => product.Title)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(product => product.VatRate)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(product => product.CommissionRate)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(product => product.LastUpdateDateTime)
                .IsRequired();

            builder.HasIndex(product => new { product.MarketplaceAccountId, product.Barcode })
                .HasDatabaseName("IX_Product_Marketplace_Barcode");

            builder.HasIndex(product => product.MarketplaceProductId);
        }
    }
}
