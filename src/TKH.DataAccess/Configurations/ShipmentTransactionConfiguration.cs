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
            builder.HasKey(shipmentTransaction => shipmentTransaction.Id);

            builder.Property(shipmentTransaction => shipmentTransaction.MarketplaceAccountId)
                .IsRequired();

            builder.Property(shipmentTransaction => shipmentTransaction.MarketplaceOrderNumber)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(shipmentTransaction => shipmentTransaction.MarketplaceParcelId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(shipmentTransaction => shipmentTransaction.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(shipmentTransaction => shipmentTransaction.Desi)
                .IsRequired();

            builder.HasIndex(shipmentTransaction => shipmentTransaction.MarketplaceOrderNumber);
            builder.HasIndex(shipmentTransaction => shipmentTransaction.MarketplaceAccountId);
            builder.HasIndex(shipmentTransaction => new { shipmentTransaction.MarketplaceAccountId, shipmentTransaction.MarketplaceParcelId })
                .IsUnique();

            builder.HasOne(shipmentTransaction => shipmentTransaction.MarketplaceAccount)
                .WithMany()
                .HasForeignKey(shipmentTransaction => shipmentTransaction.MarketplaceAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
