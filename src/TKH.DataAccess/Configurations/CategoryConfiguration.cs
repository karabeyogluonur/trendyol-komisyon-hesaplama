using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities;

namespace TKH.DataAccess.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.Property(category => category.ExternalId).IsRequired().HasMaxLength(100);
            builder.Property(category => category.ParentExternalId).HasMaxLength(100);
            builder.Property(category => category.Name).IsRequired().HasMaxLength(250);
            builder.Property(category => category.DefaultCommissionRate).HasPrecision(18, 4);
            builder.HasIndex(category => new { category.MarketplaceType, category.ExternalId }).IsUnique();
        }
    }
}
