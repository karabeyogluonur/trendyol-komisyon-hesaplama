using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Core.Utilities.Results;

namespace TKH.Business.Abstract
{
    public interface IMarketplaceAccountService
    {
        Task<IDataResult<List<MarketplaceAccountSummaryDto>>> GetAllAsync();
        Task<IDataResult<List<MarketplaceAccountSummaryDto>>> GetActiveAccountsAsync();
        Task<IResult> AddAsync(MarketplaceAccountAddDto marketplaceAccountAddDto);
        Task<IDataResult<MarketplaceAccountUpdateDto>> GetByIdAsync(int id);
        Task<IResult> UpdateAsync(MarketplaceAccountUpdateDto updateDto);
        Task<IResult> DeleteAsync(int id);

        Task<IDataResult<MarketplaceAccountConnectionDetailsDto>> GetConnectionDetailsByIdAsync(int id);
    }
}
