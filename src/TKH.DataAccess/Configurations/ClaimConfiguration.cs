using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities;

namespace TKH.DataAccess.Configurations
{
    public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
    {
        public void Configure(EntityTypeBuilder<Claim> builder)
        {
            builder.ToTable("Claims");

            builder.Property(claim => claim.ExternalId).IsRequired().HasMaxLength(100);
            builder.Property(claim => claim.ExternalOrderNumber).IsRequired().HasMaxLength(100);
            builder.Property(claim => claim.ExternalShipmentPackageId).HasMaxLength(100);

            builder.Property(claim => claim.CustomerFirstName).HasMaxLength(100);
            builder.Property(claim => claim.CustomerLastName).HasMaxLength(100);

            builder.Property(claim => claim.CargoTrackingNumber).HasMaxLength(100);
            builder.Property(claim => claim.CargoProviderName).HasMaxLength(100);
            builder.Property(claim => claim.CargoSenderNumber).HasMaxLength(100);
            builder.Property(claim => claim.CargoTrackingLink).HasMaxLength(500);

            builder.Property(claim => claim.RejectedExternalPackageId).HasMaxLength(100);
            builder.Property(claim => claim.RejectedCargoTrackingNumber).HasMaxLength(100);
            builder.Property(claim => claim.RejectedCargoProviderName).HasMaxLength(100);
            builder.Property(claim => claim.RejectedCargoTrackingLink).HasMaxLength(500);

            builder.HasIndex(claim => claim.ExternalId);
            builder.HasIndex(claim => claim.ExternalOrderNumber);
            builder.HasIndex(claim => claim.ClaimDate);
            builder.HasIndex(claim => claim.LastUpdateDateTime);

            builder.HasMany(claim => claim.ClaimItems)
                   .WithOne(claimItem => claimItem.Claim)
                   .HasForeignKey(claimItem => claimItem.ClaimId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
