using TKH.Core.Utilities.Results;
using TKH.Web.Features.ProductProfits.Models;
using TKH.Web.Features.Products.Models;

namespace TKH.Web.Features.ProductProfits.Services
{
    public interface IProductProfitOrchestrator
    {
        Task<IDataResult<ProductProfitListViewModel>> PrepareProductProfitListViewModelAsync(ProductProfitListFilterViewModel productProfitListFilterViewModel);
    }
}
