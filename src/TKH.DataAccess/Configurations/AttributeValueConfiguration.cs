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

            builder.Property(attributeValue => attributeValue.ExternalId).IsRequired().HasMaxLength(100);
            builder.Property(attributeValue => attributeValue.Value).IsRequired().HasMaxLength(500);

            builder.HasIndex(attributeValue => new { attributeValue.CategoryAttributeId, attributeValue.ExternalId }).IsUnique();
        }
    }
}
