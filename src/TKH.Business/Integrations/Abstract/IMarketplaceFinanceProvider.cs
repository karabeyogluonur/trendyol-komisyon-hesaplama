using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Business.Integrations.Dtos;

namespace TKH.Business.Integrations.Abstract
{
    public interface IMarketplaceFinanceProvider
    {
        IAsyncEnumerable<MarketplaceFinancialTransactionDto> GetFinancialTransactionsStreamAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto, CancellationToken cancellationToken = default);

        IAsyncEnumerable<MarketplaceShipmentSyncResultDto> GetShipmentTransactionsStreamAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto, CancellationToken cancellationToken = default);
    }
}
