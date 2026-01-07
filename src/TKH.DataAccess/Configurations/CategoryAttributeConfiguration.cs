using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities;

namespace TKH.DataAccess.Configurations
{
    public class CategoryAttributeConfiguration : IEntityTypeConfiguration<CategoryAttribute>
    {
        public void Configure(EntityTypeBuilder<CategoryAttribute> builder)
        {
            builder.ToTable("CategoryAttributes");

            builder.Property(categoryAttribute => categoryAttribute.ExternalId).IsRequired().HasMaxLength(100);
            builder.Property(categoryAttribute => categoryAttribute.Name).IsRequired().HasMaxLength(250);

            builder.HasIndex(categoryAttribute => new { categoryAttribute.CategoryId, categoryAttribute.ExternalId }).IsUnique();

            builder.HasMany(categoryAttribute => categoryAttribute.Values)
                   .WithOne(attributeValue => attributeValue.Attribute)
                   .HasForeignKey(attributeValue => attributeValue.CategoryAttributeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(categoryAttribute => categoryAttribute.ProductAttributes)
                   .WithOne(productAttribute => productAttribute.Attribute)
                   .HasForeignKey(productAttribute => productAttribute.CategoryAttributeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
