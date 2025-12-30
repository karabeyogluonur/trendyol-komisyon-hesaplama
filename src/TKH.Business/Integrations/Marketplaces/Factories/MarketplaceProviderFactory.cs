using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Marketplaces.Factories
{
    public class MarketplaceProviderFactory
    {
        private readonly IEnumerable<IMarketplaceProductProvider> _marketplaceProductProviders;
        private readonly IEnumerable<IMarketplaceOrderProvider> _marketplaceOrderProviders;
        private readonly IEnumerable<IMarketplaceClaimProvider> _marketplaceClaimProviders;
        private readonly IEnumerable<IMarketplaceFinanceProvider> _marketplaceFinanceProviders;
        private readonly IEnumerable<IMarketplaceCategoryProvider> _marketplaceCategoryProviders;

        public MarketplaceProviderFactory(
            IEnumerable<IMarketplaceProductProvider> marketplaceProductProviders,
            IEnumerable<IMarketplaceOrderProvider> marketplaceOrderProviders,
            IEnumerable<IMarketplaceClaimProvider> marketplaceClaimProviders,
            IEnumerable<IMarketplaceFinanceProvider> marketplaceFinanceProviders,
            IEnumerable<IMarketplaceCategoryProvider> marketplaceCategoryProviders)
        {
            _marketplaceProductProviders = marketplaceProductProviders;
            _marketplaceOrderProviders = marketplaceOrderProviders;
            _marketplaceClaimProviders = marketplaceClaimProviders;
            _marketplaceFinanceProviders = marketplaceFinanceProviders;
            _marketplaceCategoryProviders = marketplaceCategoryProviders;
        }

        public T GetProvider<T>(MarketplaceType type)
        {
            if (typeof(T) == typeof(IMarketplaceProductProvider))
            {
                IMarketplaceProductProvider? marketplaceProductProvider = _marketplaceProductProviders.FirstOrDefault(x => x.MarketplaceType == type);
                return (T)(marketplaceProductProvider ?? throw new NotSupportedException($"{type} için 'Product Provider' implementasyonu bulunamadı."));
            }

            if (typeof(T) == typeof(IMarketplaceOrderProvider))
            {
                IMarketplaceOrderProvider? marketplaceOrderProvider = _marketplaceOrderProviders.FirstOrDefault(x => x.MarketplaceType == type);
                return (T)(marketplaceOrderProvider ?? throw new NotSupportedException($"{type} için 'Order Provider' implementasyonu bulunamadı."));
            }

            if (typeof(T) == typeof(IMarketplaceClaimProvider))
            {
                IMarketplaceClaimProvider? marketplaceClaimProvider = _marketplaceClaimProviders.FirstOrDefault(x => x.MarketplaceType == type);
                return (T)(marketplaceClaimProvider ?? throw new NotSupportedException($"{type} için 'Claim Provider' implementasyonu bulunamadı."));
            }

            if (typeof(T) == typeof(IMarketplaceFinanceProvider))
            {
                IMarketplaceFinanceProvider? marketplaceFinanceProvider = _marketplaceFinanceProviders.FirstOrDefault(x => x.MarketplaceType == type);
                return (T)(marketplaceFinanceProvider ?? throw new NotSupportedException($"{type} için 'Finance Provider' implementasyonu bulunamadı."));
            }

            if (typeof(T) == typeof(IMarketplaceCategoryProvider))
            {
                IMarketplaceCategoryProvider? marketplaceCategoryProviders = _marketplaceCategoryProviders.FirstOrDefault(x => x.MarketplaceType == type);
                return (T)(marketplaceCategoryProviders ?? throw new NotSupportedException($"{type} için 'Category Provider' implementasyonu bulunamadı."));
            }

            throw new NotSupportedException($"Factory içinde '{typeof(T).Name}' tipi için bir tanımlama veya eşleştirme yapılmamış.");
        }
    }
}
