using TKH.Core.Utilities.Paging;

namespace TKH.Web.Features.Products.Models
{
    public class ProductCostListViewModel
    {
        public IPagedList<ProductCostListItemViewModel> Products { get; set; } = new PagedList<ProductCostListItemViewModel>();
        public ProductCostListFilterViewModel Filter { get; set; } = new();
    }
}
