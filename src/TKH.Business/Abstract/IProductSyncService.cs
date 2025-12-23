using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Entities;

namespace TKH.Business.Abstract
{
    public interface IProductSyncService
    {
        Task SyncProductsFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto);
    }
}
