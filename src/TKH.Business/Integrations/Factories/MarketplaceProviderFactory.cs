using Microsoft.Extensions.DependencyInjection;
using TKH.Business.Integrations.Abstract;
using TKH.Business.Integrations.Concrete;
using TKH.Entities.Enums;


namespace TKH.Business.Integrations.Factories
{
    public class MarketplaceProviderFactory(IServiceProvider serviceProvider)
    {
        public IMarketplaceProductProvider GetProvider(MarketplaceType marketplaceType)
        {
            return marketplaceType switch
            {
                MarketplaceType.Trendyol => serviceProvider.GetRequiredService<TrendyolProductProvider>(),

                _ => throw new NotImplementedException($"Bu pazaryeri ({marketplaceType}) için Provider yazılmadı!")
            };
        }
    }
}
