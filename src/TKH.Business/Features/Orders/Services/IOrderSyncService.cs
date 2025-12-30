using TKH.Business.Features.MarketplaceAccounts.Dtos;


namespace TKH.Business.Features.Orders.Services
{
    public interface IOrderSyncService
    {
        Task SyncOrdersFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto);
    }
}
