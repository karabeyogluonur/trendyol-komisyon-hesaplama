using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Marketplaces.Abstract
{
    public interface IMarketplaceFinanceProvider
    {
        MarketplaceType MarketplaceType { get; }
        IAsyncEnumerable<MarketplaceFinancialTransactionDto> GetFinancialTransactionsStreamAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto, CancellationToken cancellationToken = default);

        IAsyncEnumerable<MarketplaceShipmentSyncResultDto> GetShipmentTransactionsStreamAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto, CancellationToken cancellationToken = default);
    }
}
