using TKH.Core.Utilities.Paging;

namespace TKH.Business.Features.Products.Dtos
{
    public class ProductListFilterDto : PageRequest
    {
        public string? Barcode { get; set; }
    }
}
