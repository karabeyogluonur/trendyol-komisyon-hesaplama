using Microsoft.Extensions.DependencyInjection;
using TKH.Business.Integrations.Abstract;
using TKH.Business.Integrations.Concrete;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Factories
{
    public class MarketplaceProviderFactory(IServiceProvider serviceProvider)
    {
        public T GetProvider<T>(MarketplaceType marketplaceType)
        {
            if (typeof(T) == typeof(IMarketplaceProductProvider))
            {
                var provider = marketplaceType switch
                {
                    MarketplaceType.Trendyol => serviceProvider.GetRequiredService<TrendyolProductProvider>(),

                    _ => throw new NotImplementedException($"Bu pazaryeri ({marketplaceType}) için Product Provider yazılmadı!")
                };

                return (T)(object)provider;
            }
            if (typeof(T) == typeof(IMarketplaceOrderProvider))
            {
                var provider = marketplaceType switch
                {
                    MarketplaceType.Trendyol => serviceProvider.GetRequiredService<TrendyolOrderProvider>(),

                    _ => throw new NotImplementedException($"Bu pazaryeri ({marketplaceType}) için Order Provider yazılmadı!")
                };

                return (T)(object)provider;
            }
            if (typeof(T) == typeof(IMarketplaceClaimProvider))
            {
                var provider = marketplaceType switch
                {
                    MarketplaceType.Trendyol => serviceProvider.GetRequiredService<TrendyolClaimProvider>(),

                    _ => throw new NotImplementedException($"Bu pazaryeri ({marketplaceType}) için Order Provider yazılmadı!")
                };

                return (T)(object)provider;
            }
            if (typeof(T) == typeof(IMarketplaceFinanceProvider))
            {
                var provider = marketplaceType switch
                {
                    MarketplaceType.Trendyol => serviceProvider.GetRequiredService<TrendyolFinanceProvider>(),

                    _ => throw new NotImplementedException($"Bu pazaryeri ({marketplaceType}) için Finance Provider yazılmadı!")
                };

                return (T)(object)provider;
            }
            if (typeof(T) == typeof(IMarketplaceReferenceProvider))
            {
                var provider = marketplaceType switch
                {
                    MarketplaceType.Trendyol => serviceProvider.GetRequiredService<TrendyolReferenceProvider>(),

                    _ => throw new NotImplementedException($"Bu pazaryeri ({marketplaceType}) için Reference Provider yazılmadı!")
                };

                return (T)(object)provider;
            }

            throw new NotSupportedException($"Factory içinde '{typeof(T).Name}' tipi için tanımlama yapılmamış.");
        }
    }
}
