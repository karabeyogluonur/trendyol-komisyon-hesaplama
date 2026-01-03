using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Marketplaces.Common
{
    public interface IMarketplaceProviderBase
    {
        MarketplaceType MarketplaceType { get; }
    }
}
