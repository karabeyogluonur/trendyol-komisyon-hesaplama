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

            builder.HasOne(categoryAttribute => categoryAttribute.Category)
                   .WithMany(category => category.Attributes)
                   .HasForeignKey(categoryAttribute => categoryAttribute.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
