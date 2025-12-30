using TKH.Business.Features.MarketplaceAccounts.Dtos;

namespace TKH.Business.Features.FinancialTransactions.Services
{
    public interface IFinancialTransactionSyncService
    {
        Task SyncFinancialTransactionsFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto);
    }
}
