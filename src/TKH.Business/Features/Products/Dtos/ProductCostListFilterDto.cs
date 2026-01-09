using TKH.Business.Features.Products.Enums;
using TKH.Core.Common.Constants;
using TKH.Core.Utilities.Paging;

namespace TKH.Business.Features.Products.Dtos
{
    public class ProductCostListFilterDto : PageRequest
    {
        public string? Barcode { get; set; }
        public bool? IsOnSale { get; set; }
        public bool? HasStock { get; set; }
        public int? CategoryId { get; set; }
        public ProductCostFilterType? CostStatus { get; set; }

    }
}
