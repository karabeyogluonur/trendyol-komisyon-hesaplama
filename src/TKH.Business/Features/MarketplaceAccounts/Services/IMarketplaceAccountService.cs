using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Core.Utilities.Results;

namespace TKH.Business.Features.MarketplaceAccounts.Services
{
    public interface IMarketplaceAccountService
    {
        Task<IDataResult<List<MarketplaceAccountSummaryDto>>> GetAllAsync();
        Task<IDataResult<List<MarketplaceAccountSummaryDto>>> GetActiveAccountsAsync();
        Task<IDataResult<int>> AddAsync(MarketplaceAccountAddDto marketplaceAccountAddDto);
        Task<IDataResult<MarketplaceAccountDetailsDto>> GetByIdAsync(int id);
        Task<IResult> UpdateAsync(MarketplaceAccountUpdateDto updateDto);
        Task<IResult> DeleteAsync(int id);
        Task<IDataResult<MarketplaceAccountConnectionDetailsDto>> GetConnectionDetailsByIdAsync(int marketplaceAccountId);
        Task<IDataResult<IList<int>>> GetActiveConnectedAccountIdsAsync();
        Task<bool> TryMarkAsSyncingAsync(int marketplaceAccountId);
        Task MarkSyncCompletedAsync(int marketplaceAccountId);
        Task MarkSyncFailedAsync(int marketplaceAccountId, Exception exception);
    }
}
