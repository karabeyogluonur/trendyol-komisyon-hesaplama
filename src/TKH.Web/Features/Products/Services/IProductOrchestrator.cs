using TKH.Core.Utilities.Results;
using TKH.Web.Features.Products.Models;

namespace TKH.Web.Features.Products.Services
{
    public interface IProductOrchestrator
    {
        Task<IDataResult<ProductListViewModel>> PrepareProductListViewModelAsync(ProductListFilterViewModel productListFilterViewModel);
    }
}
