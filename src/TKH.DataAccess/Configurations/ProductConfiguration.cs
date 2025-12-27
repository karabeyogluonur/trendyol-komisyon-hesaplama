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

            builder.Property(product => product.ExternalId).HasMaxLength(64);
            builder.Property(product => product.ExternalProductCode).HasMaxLength(64);
            builder.Property(product => product.ExternalUrl).HasMaxLength(1000);
            builder.Property(product => product.Barcode).HasMaxLength(100);
            builder.Property(product => product.Sku).HasMaxLength(100);
            builder.Property(product => product.ModelCode).HasMaxLength(100);
            builder.Property(product => product.Name).IsRequired().HasMaxLength(500);
            builder.Property(product => product.ImageUrl).HasMaxLength(1000);
            builder.Property(product => product.VatRate).HasPrecision(18, 2);
            builder.Property(product => product.CommissionRate).HasPrecision(18, 2);
            builder.HasIndex(product => product.ExternalId);
            builder.HasIndex(product => product.ExternalProductCode);
            builder.HasIndex(product => product.Barcode);
            builder.HasIndex(product => product.Sku);
            builder.HasIndex(product => product.MarketplaceAccountId);
            builder.HasOne(product => product.Category)
                   .WithMany(category => category.Products)
                   .HasForeignKey(product => product.CategoryId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
