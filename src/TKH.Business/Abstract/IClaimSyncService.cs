using TKH.Business.Dtos.MarketplaceAccount;

namespace TKH.Business.Abstract
{
    public interface IClaimSyncService
    {
        Task SyncClaimsFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto);
    }
}
