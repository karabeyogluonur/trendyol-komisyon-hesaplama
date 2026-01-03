using TKH.Core.Utilities.Paging;

namespace TKH.Web.Features.Products.Models
{
    public class ProductListViewModel
    {
        public IPagedList<ProductListItemViewModel> Products { get; set; } = new PagedList<ProductListItemViewModel>();
        public ProductListFilterViewModel Filter { get; set; } = new();
    }
}
