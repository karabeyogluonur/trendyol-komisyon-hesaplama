using TKH.Business.Features.MarketplaceAccounts.Dtos;

namespace TKH.Business.Features.Claims.Services
{
    public interface IClaimSyncService
    {
        Task SyncClaimsFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto);
    }
}
