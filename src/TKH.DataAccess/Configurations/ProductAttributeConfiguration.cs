using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities;

namespace TKH.DataAccess.Configurations
{
    public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {
            builder.ToTable("ProductAttributes");

            builder.Property(productAttribute => productAttribute.CustomValue).HasMaxLength(500);

            builder.HasOne(productAttribute => productAttribute.Product)
                   .WithMany(product => product.Attributes)
                   .HasForeignKey(productAttribute => productAttribute.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(productAttribute => productAttribute.Attribute)
                   .WithMany(categoryAttribute => categoryAttribute.ProductAttributes)
                   .HasForeignKey(productAttribute => productAttribute.CategoryAttributeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
