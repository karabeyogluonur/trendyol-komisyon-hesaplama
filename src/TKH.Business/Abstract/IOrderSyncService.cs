using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Entities;

namespace TKH.Business.Abstract
{
    public interface IOrderSyncService
    {
        Task SyncOrdersFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto);
    }
}
