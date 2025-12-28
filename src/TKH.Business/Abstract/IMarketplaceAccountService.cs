using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Core.Utilities.Results;

namespace TKH.Business.Abstract
{
    public interface IMarketplaceAccountService
    {
        Task<IDataResult<List<MarketplaceAccountSummaryDto>>> GetAllAsync();
        Task<IDataResult<List<MarketplaceAccountSummaryDto>>> GetActiveAccountsAsync();
        Task<IDataResult<int>> AddAsync(MarketplaceAccountAddDto marketplaceAccountAddDto);
        Task<IDataResult<MarketplaceAccountDetailsDto>> GetByIdAsync(int id);
        Task<IResult> UpdateAsync(MarketplaceAccountUpdateDto updateDto);
        Task<IResult> DeleteAsync(int id);
        Task<IDataResult<MarketplaceAccountConnectionDetailsDto>> GetConnectionDetailsByIdAsync(int id);
        Task<IDataResult<MarketplaceAccountConnectionDetailsDto>> GetConnectionDetailsAsync(int accountId);
        Task<IDataResult<IList<int>>> GetActiveConnectedAccountIdsAsync();
        void MarkAsSyncing(int accountId);
        void MarkAsIdle(int accountId, Exception? exception);
    }
}
