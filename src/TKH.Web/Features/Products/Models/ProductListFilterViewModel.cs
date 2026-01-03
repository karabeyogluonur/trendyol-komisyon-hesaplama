using Microsoft.AspNetCore.Mvc.Rendering;
using TKH.Core.Common.Constants;

namespace TKH.Web.Features.Products.Models
{
    public class ProductListFilterViewModel
    {
        public string? Barcode { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = ApplicationDefaults.ProductPageSize;
        public bool? IsOnSale { get; set; }
        public bool? HasStock { get; set; }
        public int? CategoryId { get; set; }

        public List<SelectListItem> Categories { get; set; }
    }
}
