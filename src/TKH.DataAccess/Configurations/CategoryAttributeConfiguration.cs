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

            builder.Property(categoryAttribute => categoryAttribute.MarketplaceAttributeId).HasMaxLength(100).IsRequired();
            builder.Property(categoryAttribute => categoryAttribute.Name).HasMaxLength(250);

            builder.HasOne(categoryAttribute => categoryAttribute.Category)
                   .WithMany(category => category.CategoryAttributes)
                   .HasForeignKey(categoryAttribute => categoryAttribute.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(categoryAttribute => new { categoryAttribute.CategoryId, categoryAttribute.MarketplaceAttributeId })
                   .IsUnique();
        }
    }
}
