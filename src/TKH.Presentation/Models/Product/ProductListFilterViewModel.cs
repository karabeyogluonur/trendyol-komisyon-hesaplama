using TKH.Core.Common.Constants;

namespace TKH.Presentation.Models.Product
{
    public class ProductListFilterViewModel
    {
        public string? Barcode { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = ApplicationDefaults.ProductPageSize;
    }
}
