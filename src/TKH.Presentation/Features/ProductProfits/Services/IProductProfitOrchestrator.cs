using TKH.Core.Utilities.Results;
using TKH.Presentation.Features.ProductProfits.Models;
using TKH.Presentation.Features.Products.Models;

namespace TKH.Presentation.Features.ProductProfits.Services
{
    public interface IProductProfitOrchestrator
    {
        Task<IDataResult<ProductProfitListViewModel>> PrepareProductProfitListViewModelAsync(ProductProfitListFilterViewModel productProfitListFilterViewModel);
    }
}
