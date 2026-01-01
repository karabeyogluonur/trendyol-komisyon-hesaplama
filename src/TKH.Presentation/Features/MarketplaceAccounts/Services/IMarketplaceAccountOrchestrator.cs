using TKH.Core.Utilities.Results;
using TKH.Presentation.Features.MarketplaceAccounts.Models;
using IResult = TKH.Core.Utilities.Results.IResult;

namespace TKH.Presentation.Features.MarketplaceAccounts.Services
{
    public interface IMarketplaceAccountOrchestrator
    {
        Task<IDataResult<List<MarketplaceAccountListViewModel>>> PrepareMarketplaceAccountListViewModelAsync();

        MarketplaceAccountAddViewModel PrepareMarketplaceAccountAddViewModel();

        Task<IResult> CreateMarketplaceAccountAsync(MarketplaceAccountAddViewModel marketplaceAccountAddViewModel);

        Task<IDataResult<MarketplaceAccountUpdateViewModel>> PrepareMarketplaceAccountUpdateViewModelAsync(int marketplaceAccountId);

        Task<IResult> UpdateMarketplaceAccountAsync(MarketplaceAccountUpdateViewModel marketplaceAccountAddViewModel);

        Task<IResult> DeleteMarketplaceAccountAsync(int marketplaceAccountId);
    }
}
