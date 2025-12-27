using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities;

namespace TKH.DataAccess.Configurations
{
    public class ShipmentTransactionConfiguration : IEntityTypeConfiguration<ShipmentTransaction>
    {
        public void Configure(EntityTypeBuilder<ShipmentTransaction> builder)
        {
            builder.ToTable("ShipmentTransactions");

            builder.Property(shipmentTransaction => shipmentTransaction.ExternalOrderNumber).IsRequired().HasMaxLength(100);
            builder.Property(shipmentTransaction => shipmentTransaction.ExternalParcelId).HasMaxLength(100);

            builder.Property(shipmentTransaction => shipmentTransaction.Amount).HasPrecision(18, 2);

            builder.HasOne(shipmentTransaction => shipmentTransaction.MarketplaceAccount)
                   .WithMany(account => account.ShipmentTransactions)
                   .HasForeignKey(shipmentTransaction => shipmentTransaction.MarketplaceAccountId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
