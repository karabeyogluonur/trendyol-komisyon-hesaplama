using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Entities.Enums;
using TKH.Integrations.Trendyol.Settings;

namespace TKH.Integrations.Trendyol.Providers
{
    public class TrendyolDefaultsProvider(TrendyolSettings trendyolSettings) : IMarketplaceDefaultsProvider
    {
        public MarketplaceType MarketplaceType => MarketplaceType.Trendyol;

        public MarketplaceDefaultsDto GetDefaults()
        {
            return new MarketplaceDefaultsDto
            {
                ServiceFee = trendyolSettings.ServiceFeeAmount,
                ServiceFeeVatRate = trendyolSettings.ServiceFeeVatRate,

                Metadata = new Dictionary<string, object>
                {
                    { "SameDayServiceFee", trendyolSettings.SameDayServiceFeeAmount },
                    { "ExportServiceFeeRate", trendyolSettings.ExportServiceFeeRate },
                    { "ProductCommissionVatRate", trendyolSettings.ProductCommissionVatRate },
                    { "ExportServiceFeeVatRate", trendyolSettings.ExportServiceFeeVatRate }
                }
            };
        }
    }
}
