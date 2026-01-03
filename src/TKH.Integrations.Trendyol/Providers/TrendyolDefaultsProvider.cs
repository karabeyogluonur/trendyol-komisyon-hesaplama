using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Business.Integrations.Providers.Trendyol;
using TKH.Entities.Enums;

namespace TKH.Integrations.Trendyol.Providers
{
    public class TrendyolDefaultsProvider : IMarketplaceDefaultsProvider
    {
        public MarketplaceType MarketplaceType => MarketplaceType.Trendyol;

        public MarketplaceDefaultsDto GetDefaults()
        {
            return new MarketplaceDefaultsDto
            {
                ServiceFee = TrendyolDefaults.FixedServiceFeeAmount,
                Metadata = new Dictionary<string, object>
                {
                    { "SameDayServiceFee", TrendyolDefaults.FixedSameDayServiceFeeAmount },
                }
            };
        }
    }
}
