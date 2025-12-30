using TKH.Entities.Enums;

namespace TKH.Business.Features.Categories.Services
{
    public interface ICategorySyncService
    {
        Task SyncCategoriesAsync(MarketplaceType marketplaceType);
        Task SyncCategoryAttributesAsync(MarketplaceType marketplaceType);
    }
}
