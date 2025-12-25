using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities;

namespace TKH.DataAccess.Configurations
{
    public class ProductPriceConfiguration : IEntityTypeConfiguration<ProductPrice>
    {
        public void Configure(EntityTypeBuilder<ProductPrice> builder)
        {
            builder.ToTable("ProductPrices");

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Type)
                .IsRequired();

            builder.Property(x => x.IsVatIncluded)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.StartDate)
                .IsRequired();

            builder.Property(x => x.EndDate)
                .IsRequired(false);

            builder.HasOne(x => x.Product)
                .WithMany(p => p.ProductPrices)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.ProductId, x.Type, x.EndDate })
                .HasDatabaseName("IX_ProductPrices_ActiveLookup");

            builder.HasIndex(x => new { x.ProductId, x.Type, x.StartDate })
                .HasDatabaseName("IX_ProductPrices_HistoryLookup");
        }
    }
}
