using TKH.Business.Features.Categories.Dtos;
using TKH.Business.Features.Products.Dtos;
using TKH.Core.Utilities.Paging;
using TKH.Core.Utilities.Results;

namespace TKH.Business.Features.Products.Services
{
    public interface IProductService
    {
        Task<IDataResult<IPagedList<ProductSummaryDto>>> GetPagedProductListAsync(ProductListFilterDto productListFilterDto);
        Task<IDataResult<List<CategoryLookupDto>>> GetUsedCategoriesAsync();
        Task<IDataResult<IPagedList<ProductProfitSummaryDto>>> GetPagedProductProfitListAsync(ProductProfitListFilterDto productProfitListFilterDto);
        Task<IDataResult<IPagedList<ProductCostSummaryDto>>> GetPagedProductCostListAsync(ProductCostListFilterDto productCostListFilterDto);
        Task<IDataResult<ProductSummaryDto>> GetProductByIdAsync(int productId);
        Task<IDataResult<List<ProductSummaryDto>>> GetProductsByIdsAsync(IEnumerable<int> productIds);

    }
}
