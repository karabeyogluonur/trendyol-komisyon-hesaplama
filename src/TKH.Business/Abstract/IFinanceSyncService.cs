using TKH.Business.Dtos.MarketplaceAccount;

namespace TKH.Business.Abstract
{
    public interface IFinanceSyncService
    {
        Task SyncFinancialTransactionsFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto);
    }
}
