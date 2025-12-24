using TKH.Entities.Enums;

namespace TKH.Business.Abstract
{
    public interface IMarketplaceReferenceSyncService
    {
        Task SyncCategoriesAsync(MarketplaceType marketplaceType);
        Task SyncCategoryAttributesAsync(MarketplaceType marketplaceType);
    }
}
