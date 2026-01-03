using TKH.Business.Integrations.Marketplaces.Common;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Marketplaces.Abstract
{
    public interface IMarketplaceDefaultsProvider : IMarketplaceProviderBase
    {
        MarketplaceDefaultsDto GetDefaults();
    }
}
