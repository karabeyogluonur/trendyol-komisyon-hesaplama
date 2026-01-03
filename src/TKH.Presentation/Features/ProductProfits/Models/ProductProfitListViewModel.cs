using TKH.Core.Utilities.Paging;
using TKH.Entities.Enums;
using TKH.Presentation.Features.Common.Models;
using TKH.Presentation.Features.ProductProfits.Models;

namespace TKH.Presentation.Features.Products.Models
{
    public class ProductProfitListViewModel
    {
        public IPagedList<ProductProfitItemViewModel> Products { get; set; }
        public ProductProfitListFilterViewModel Filter { get; set; }
        public MarketplaceType MarketplaceType { get; set; }
        public MarketplaceDefaultsViewModel MarketplaceDefaults { get; set; }
        public decimal WithholdingRate { get; set; }
        public decimal ShippingVatRate { get; set; }
    }
}
