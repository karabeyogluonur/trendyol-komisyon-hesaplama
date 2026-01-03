using TKH.Core.Utilities.Paging;

namespace TKH.Business.Features.Products.Dtos
{
    public class ProductProfitListFilterDto : PageRequest
    {
        public string? Barcode { get; set; }
        public bool? IsOnSale { get; set; }
        public bool? HasStock { get; set; }
        public int? CategoryId { get; set; }
    }
}
