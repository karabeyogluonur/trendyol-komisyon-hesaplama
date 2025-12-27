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

            builder.HasMany(marketplaceAccount => marketplaceAccount.Products)
                   .WithOne(marketplaceAccount => marketplaceAccount.MarketplaceAccount)
                   .HasForeignKey(marketplaceAccount => marketplaceAccount.MarketplaceAccountId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(marketplaceAccount => marketplaceAccount.Orders)
                   .WithOne(marketplaceAccount => marketplaceAccount.MarketplaceAccount)
                   .HasForeignKey(marketplaceAccount => marketplaceAccount.MarketplaceAccountId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(marketplaceAccount => marketplaceAccount.ShipmentTransactions)
                  .WithOne(marketplaceAccount => marketplaceAccount.MarketplaceAccount)
                  .HasForeignKey(marketplaceAccount => marketplaceAccount.MarketplaceAccountId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
