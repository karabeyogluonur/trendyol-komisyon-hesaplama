using TKH.Business.Integrations.Marketplaces.Common;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Marketplaces.Abstract
{
    public interface IMarketplaceCategoryProvider : IMarketplaceProviderBase
    {
        Task<List<MarketplaceCategoryDto>> GetCategoryTreeAsync(CancellationToken cancellationToken = default);
        Task<List<MarketplaceCategoryAttributeDto>> GetCategoryAttributesAsync(string marketplaceCategoryId, CancellationToken cancellationToken = default);
    }
}
