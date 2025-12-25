using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities;

namespace TKH.DataAccess.Configurations
{
    public class AttributeValueConfiguration : IEntityTypeConfiguration<AttributeValue>
    {
        public void Configure(EntityTypeBuilder<AttributeValue> builder)
        {
            builder.ToTable("AttributeValues");

            builder.Property(attributeValue => attributeValue.MarketplaceValueId).HasMaxLength(100).IsRequired();
            builder.Property(attributeValue => attributeValue.Value).HasMaxLength(int.MaxValue);

            builder.HasOne(attributeValue => attributeValue.CategoryAttribute)
                   .WithMany(categoryAttribute => categoryAttribute.AttributeValues)
                   .HasForeignKey(attributeValue => attributeValue.CategoryAttributeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(attributeValue => new { attributeValue.CategoryAttributeId, attributeValue.MarketplaceValueId })
                   .IsUnique();
        }
    }
}
