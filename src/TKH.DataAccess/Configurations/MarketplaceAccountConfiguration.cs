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

            builder.HasKey(marketplaceAccount => marketplaceAccount.Id);
            builder.Property(marketplaceAccount => marketplaceAccount.StoreName).IsRequired().HasMaxLength(100);
            builder.Property(marketplaceAccount => marketplaceAccount.ApiKey).IsRequired().HasMaxLength(100);
            builder.Property(marketplaceAccount => marketplaceAccount.ApiSecretKey).IsRequired();
            builder.Property(marketplaceAccount => marketplaceAccount.MerchantId).IsRequired().HasMaxLength(50);
            builder.Property(marketplaceAccount => marketplaceAccount.MarketplaceType).IsRequired();
        }
    }
}
