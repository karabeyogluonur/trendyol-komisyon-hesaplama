using TKH.Business.Integrations.Dtos;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Abstract
{
    public interface IMarketplaceReferenceProvider
    {
        Task<List<MarketplaceCategoryDto>> GetCategoryTreeAsync(CancellationToken cancellationToken = default);
        Task<List<MarketplaceCategoryAttributeDto>> GetCategoryAttributesAsync(string marketplaceCategoryId, CancellationToken cancellationToken = default);
    }
}
