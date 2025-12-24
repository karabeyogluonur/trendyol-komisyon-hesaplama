using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities;

public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.ToTable("ProductAttributes");

        builder.HasIndex(productAttribute => productAttribute.ProductId);
        builder.HasIndex(productAttribute => productAttribute.MarketplaceAttributeId);

        builder.Property(productAttribute => productAttribute.MarketplaceAttributeId).HasMaxLength(100).IsRequired();
        builder.Property(productAttribute => productAttribute.AttributeName).HasMaxLength(250);
        builder.Property(productAttribute => productAttribute.Value).HasMaxLength(int.MaxValue);

        builder.HasOne(productAttribute => productAttribute.Product)
            .WithMany(p => p.ProductAttributes)
            .HasForeignKey(productAttribute => productAttribute.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
