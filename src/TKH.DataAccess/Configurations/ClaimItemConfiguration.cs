using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities;

namespace TKH.DataAccess.Configurations
{
    public class ClaimItemConfiguration : IEntityTypeConfiguration<ClaimItem>
    {
        public void Configure(EntityTypeBuilder<ClaimItem> builder)
        {
            builder.ToTable("ClaimItems");

            builder.Property(claimItem => claimItem.ExternalId).IsRequired().HasMaxLength(100);
            builder.Property(claimItem => claimItem.ExternalOrderLineItemId).HasMaxLength(100);
            builder.Property(claimItem => claimItem.Barcode).HasMaxLength(100);
            builder.Property(claimItem => claimItem.Sku).HasMaxLength(100);
            builder.Property(claimItem => claimItem.ProductName).HasMaxLength(500);

            builder.Property(claimItem => claimItem.Price).HasPrecision(18, 2);
            builder.Property(claimItem => claimItem.VatRate).HasPrecision(18, 2);

            builder.Property(claimItem => claimItem.ReasonCode).HasMaxLength(50);
            builder.Property(claimItem => claimItem.ReasonName).HasMaxLength(200);
            builder.Property(claimItem => claimItem.CustomerNote).HasMaxLength(1000);

            builder.HasIndex(claimItem => claimItem.ExternalId);
            builder.HasIndex(claimItem => claimItem.Status);

            builder.Property(claimItem => claimItem.ReasonType).HasConversion<int>().IsRequired();

            builder.HasOne(claimItem => claimItem.Product)
                   .WithMany()
                   .HasForeignKey(claimItem => claimItem.ProductId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
