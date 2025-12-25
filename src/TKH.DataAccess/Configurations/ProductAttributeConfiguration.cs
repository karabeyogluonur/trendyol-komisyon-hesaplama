using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities;

public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.ToTable("ProductAttributes");

        builder.HasOne(productAttribute => productAttribute.Product)
               .WithMany(product => product.ProductAttributes)
               .HasForeignKey(productAttribute => productAttribute.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(productAttribute => productAttribute.CategoryAttribute)
               .WithMany()
               .HasForeignKey(productAttribute => productAttribute.CategoryAttributeId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(productAttribute => productAttribute.AttributeValue)
               .WithMany()
               .HasForeignKey(productAttribute => productAttribute.AttributeValueId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(productAttribute => new { productAttribute.ProductId, productAttribute.CategoryAttributeId })
               .IsUnique();
    }
}
