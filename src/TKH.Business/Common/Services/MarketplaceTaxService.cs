using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Business.Integrations.Marketplaces.Factories;
using TKH.Entities.Enums;

namespace TKH.Business.Common.Services
{
    public class MarketplaceTaxService : IMarketplaceTaxService
    {
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;
        private const string MetadataKey_ProductCommissionVatRate = "ProductCommissionVatRate";

        public MarketplaceTaxService(MarketplaceProviderFactory marketplaceProviderFactory)
        {
            _marketplaceProviderFactory = marketplaceProviderFactory;
        }

        public decimal GetVatRateByExpenseType(MarketplaceType marketplaceType, ProductExpenseType expenseType)
        {
            IMarketplaceDefaultsProvider? marketplaceDefaultsProvider = _marketplaceProviderFactory.GetProvider<IMarketplaceDefaultsProvider>(marketplaceType);

            if (marketplaceDefaultsProvider is null)
                return 0;

            MarketplaceDefaultsDto marketplaceDefaultsDto = marketplaceDefaultsProvider.GetDefaults();

            switch (expenseType)
            {
                case ProductExpenseType.MarketplaceServiceFee:
                    return marketplaceDefaultsDto.ServiceFeeVatRate;

                case ProductExpenseType.CommissionRate:
                    return GetValueFromMetadata(marketplaceDefaultsDto.Metadata, MetadataKey_ProductCommissionVatRate);

                default:
                    return 0;
            }
        }

        private decimal GetValueFromMetadata(Dictionary<string, object> metadata, string key)
        {
            if (metadata.TryGetValue(key, out object? value) && value != null)
            {
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return 0;
                }
            }
            return 0;
        }
    }
}
