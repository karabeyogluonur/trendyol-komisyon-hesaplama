using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Common;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Marketplaces.Factories
{
    public class MarketplaceProviderFactory
    {
        private readonly Dictionary<Type, Func<MarketplaceType, object>> _providerStrategies;

        public MarketplaceProviderFactory(
            IEnumerable<IMarketplaceProductProvider> productProviders,
            IEnumerable<IMarketplaceOrderProvider> orderProviders,
            IEnumerable<IMarketplaceClaimProvider> claimProviders,
            IEnumerable<IMarketplaceFinanceProvider> financeProviders,
            IEnumerable<IMarketplaceCategoryProvider> categoryProviders,
            IEnumerable<IMarketplaceDefaultsProvider> defaultsProviders)
        {
            _providerStrategies = new Dictionary<Type, Func<MarketplaceType, object>>
            {
                { typeof(IMarketplaceProductProvider), marketplaceType => FindProvider(productProviders, marketplaceType) },
                { typeof(IMarketplaceOrderProvider), marketplaceType => FindProvider(orderProviders, marketplaceType) },
                { typeof(IMarketplaceClaimProvider), marketplaceType => FindProvider(claimProviders, marketplaceType) },
                { typeof(IMarketplaceFinanceProvider), marketplaceType => FindProvider(financeProviders, marketplaceType) },
                { typeof(IMarketplaceCategoryProvider), marketplaceType => FindProvider(categoryProviders, marketplaceType) },
                { typeof(IMarketplaceDefaultsProvider), marketplaceType => FindProvider(defaultsProviders, marketplaceType) }
            };
        }

        public T GetProvider<T>(MarketplaceType marketplaceType) where T : class
        {
            if (_providerStrategies.TryGetValue(typeof(T), out var strategy))
            {
                Object provider = strategy(marketplaceType);
                return (T)provider;
            }

            throw new NotSupportedException($"Factory hatası: '{typeof(T).Name}' tipi için bir strateji tanımlanmamış.");
        }

        private static TProvider FindProvider<TProvider>(IEnumerable<TProvider> providers, MarketplaceType marketplaceType) where TProvider : IMarketplaceProviderBase
        {
            TProvider provider = providers.FirstOrDefault(provider => provider.MarketplaceType == marketplaceType);

            if (provider is null)
                throw new InvalidOperationException($"'{typeof(TProvider).Name}' için '{marketplaceType}' pazar yeri implementasyonu bulunamadı.");

            return provider;
        }
    }
}
