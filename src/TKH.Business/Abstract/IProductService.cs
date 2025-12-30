using TKH.Business.Dtos.Product;
using TKH.Core.Utilities.Paging;
using TKH.Core.Utilities.Results;

namespace TKH.Business.Abstract
{
    public interface IProductService
    {
        Task<IDataResult<IPagedList<ProductSummaryDto>>> GetPagedListAsync(ProductListFilterDto productListFilterDto);
    }
}
