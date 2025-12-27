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
            builder.HasKey(product => product.Id);
            builder.Property(product => product.MarketplaceProductId)
                .IsRequired()
                .HasMaxLength(64);

            builder.HasIndex(product => product.MarketplaceProductId);
            builder.Property(product => product.Barcode)
                .HasMaxLength(64);

            builder.HasIndex(product => product.Barcode);
            builder.Property(product => product.ModelCode).HasMaxLength(100);
            builder.Property(product => product.StockCode).HasMaxLength(100);
            builder.Property(product => product.Name)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(product => product.MarketplaceProductUrl).HasMaxLength(500);
            builder.Property(product => product.ImageUrl).HasMaxLength(500);
            builder.Property(product => product.CargoDeci).IsRequired();
            builder.Property(product => product.VatRate).HasPrecision(18, 2);
            builder.Property(product => product.CommissionRate).HasPrecision(18, 2);
            builder.Property(product => product.StockQuantity).IsRequired();
            builder.Property(product => product.UnitType).IsRequired();

            builder.HasOne(product => product.MarketplaceAccount)
                .WithMany()
                .HasForeignKey(product => product.MarketplaceAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(product => product.Category)
                .WithMany()
                .HasForeignKey(product => product.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(product => product.OrderItems)
                .WithOne(orderItem => orderItem.Product)
                .HasForeignKey(orderItem => orderItem.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
