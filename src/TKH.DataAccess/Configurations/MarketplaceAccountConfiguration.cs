using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities;

namespace TKH.DataAccess.Configurations
{
    public class MarketplaceAccountConfiguration : IEntityTypeConfiguration<MarketplaceAccount>
    {
        public void Configure(EntityTypeBuilder<MarketplaceAccount> builder)
        {
            builder.ToTable("MarketplaceAccounts");

            builder.Property(marketplaceAccount => marketplaceAccount.ApiKey).HasMaxLength(500).IsRequired();
            builder.Property(marketplaceAccount => marketplaceAccount.ApiSecretKey).HasMaxLength(500).IsRequired();
            builder.Property(marketplaceAccount => marketplaceAccount.StoreName).HasMaxLength(200).IsRequired();
            builder.Property(marketplaceAccount => marketplaceAccount.MerchantId).HasMaxLength(100).IsRequired();

            builder.Property(marketplaceAccount => marketplaceAccount.IsActive)
                   .HasDefaultValue(true);

            builder.Property(marketplaceAccount => marketplaceAccount.ConnectionState)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(marketplaceAccount => marketplaceAccount.LastErrorMessage)
                   .HasMaxLength(2000)
                   .IsRequired(false);

            builder.Property(marketplaceAccount => marketplaceAccount.LastErrorDate)
                   .IsRequired(false);

            builder.Property(marketplaceAccount => marketplaceAccount.SyncState)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(marketplaceAccount => marketplaceAccount.LastSyncStartTime)
                   .IsRequired(false);


            builder.HasMany(marketplaceAccount => marketplaceAccount.Products)
                   .WithOne(product => product.MarketplaceAccount)
                   .HasForeignKey(product => product.MarketplaceAccountId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(marketplaceAccount => marketplaceAccount.Orders)
                   .WithOne(order => order.MarketplaceAccount)
                   .HasForeignKey(order => order.MarketplaceAccountId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(marketplaceAccount => marketplaceAccount.ShipmentTransactions)
                   .WithOne(shipmentTransaction => shipmentTransaction.MarketplaceAccount)
                   .HasForeignKey(shipmentTransaction => shipmentTransaction.MarketplaceAccountId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
