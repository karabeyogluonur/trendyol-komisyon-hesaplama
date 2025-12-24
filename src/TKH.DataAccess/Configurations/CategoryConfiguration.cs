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
            builder.Property(category => category.MarketplaceCategoryId).HasMaxLength(100).IsRequired();
            builder.Property(category => category.Name).HasMaxLength(250).IsRequired();
        }
    }
}
