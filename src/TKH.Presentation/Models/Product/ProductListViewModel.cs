using Microsoft.AspNetCore.Mvc.Rendering;
using TKH.Core.Utilities.Paging;

namespace TKH.Presentation.Models.Product
{
    public class ProductListViewModel
    {
        public IPagedList<ProductListItemViewModel> Products { get; set; } = new PagedList<ProductListItemViewModel>();
        public ProductListFilterViewModel Filter { get; set; } = new();
    }
}
