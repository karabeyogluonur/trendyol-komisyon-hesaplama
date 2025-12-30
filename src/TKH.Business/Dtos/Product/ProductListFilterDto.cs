using TKH.Core.Utilities.Paging;

namespace TKH.Business.Dtos.Product
{
    public class ProductListFilterDto : PageRequest
    {
        public string? Barcode { get; set; }
    }
}
