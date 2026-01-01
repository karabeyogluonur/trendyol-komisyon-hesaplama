using TKH.Core.Utilities.Results;
using TKH.Presentation.Features.Products.Models;

namespace TKH.Presentation.Features.Products.Services
{
    public interface IProductOrchestrator
    {
        Task<IDataResult<ProductListViewModel>> PrepareProductListViewModelAsync(ProductListFilterViewModel productListFilterViewModel);
    }
}
