using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities.Concrete;

namespace TKH.DataAccess.Configurations
{
    public class MarketplaceAccountConfiguration : IEntityTypeConfiguration<MarketplaceAccount>
    {
        public void Configure(EntityTypeBuilder<MarketplaceAccount> builder)
        {
            builder.ToTable("MarketplaceAccounts");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.StoreName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ApiKey).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ApiSecretKey).IsRequired();
            builder.Property(x => x.MerchantId).IsRequired().HasMaxLength(50);
            builder.Property(x => x.MarketplaceType).IsRequired();
        }
    }
}
