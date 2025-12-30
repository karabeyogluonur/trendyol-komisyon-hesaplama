using TKH.Business.Features.MarketplaceAccounts.Dtos;

namespace TKH.Business.Features.ShipmentTransactions.Services
{
    public interface IShipmentTransactionSyncService
    {
        Task SyncShipmentTransactionsFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto);
    }
}
