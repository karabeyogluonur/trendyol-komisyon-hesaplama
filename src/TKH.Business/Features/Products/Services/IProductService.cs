using TKH.Business.Features.Categories.Dtos;
using TKH.Business.Features.Products.Dtos;
using TKH.Core.Utilities.Paging;
using TKH.Core.Utilities.Results;

namespace TKH.Business.Features.Products.Services
{
    public interface IProductService
    {
        Task<IDataResult<IPagedList<ProductSummaryDto>>> GetPagedListAsync(ProductListFilterDto productListFilterDto);
        Task<IDataResult<List<CategoryLookupDto>>> GetUsedCategoriesAsync();

    }
}
