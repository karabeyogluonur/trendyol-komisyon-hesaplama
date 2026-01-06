using TKH.Core.Utilities.Results;
using TKH.Web.Features.Products.Models;
using IResult = TKH.Core.Utilities.Results.IResult;

namespace TKH.Web.Features.Products.Services
{
    public interface IProductOrchestrator
    {
        Task<IDataResult<ProductListViewModel>> PrepareProductListViewModelAsync(ProductListFilterViewModel productListFilterViewModel);
        Task<IDataResult<ProductCostListViewModel>> PrepareProductCostListViewModelAsync(ProductCostListFilterViewModel productCostListFilterViewModel);
        Task<IResult> UpdateProductCostsAsync(List<ProductCostBatchViewModel> productCostBatchViewModels);
    }
}
