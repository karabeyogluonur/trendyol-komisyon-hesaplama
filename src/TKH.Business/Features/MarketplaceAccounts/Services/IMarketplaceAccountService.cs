using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Core.Utilities.Results;

namespace TKH.Business.Features.MarketplaceAccounts.Services
{
    public interface IMarketplaceAccountService
    {
        Task<IDataResult<List<MarketplaceAccountSummaryDto>>> GetAllMarketplaceAccountsAsync();
        Task<IDataResult<List<MarketplaceAccountSummaryDto>>> GetActiveMarketplaceAccountsAsync();
        Task<IDataResult<int>> CreateMarketplaceAccountAsync(MarketplaceAccountCreateDto marketplaceAccountCreateDto);
        Task<IDataResult<MarketplaceAccountDetailsDto>> GetMarketplaceAccountByIdAsync(int marketplaceAccountId);
        Task<IResult> UpdateMarketplaceAccountAsync(MarketplaceAccountUpdateDto marketplaceAccountUpdateDto);
        Task<IResult> DeleteMarketplaceAccountByIdAsync(int marketplaceAccountId);
        Task<IDataResult<MarketplaceAccountConnectionDetailsDto>> GetConnectionDetailsByIdAsync(int marketplaceAccountId);
        Task<IDataResult<IList<int>>> GetActiveConnectedMarketplaceAccountIdsAsync();
        Task<bool> TryMarkMarketplaceAccountAsSyncingAsync(int marketplaceAccountId);
        Task MarkMarketplaceAccountSyncCompletedAsync(int marketplaceAccountId);
        Task MarkMarketplaceAccountSyncFailedAsync(int marketplaceAccountId, Exception exception);
    }
}
