using System.ComponentModel.DataAnnotations;
using TKH.Core.Utilities.Paging;
using TKH.Entities.Enums;
using TKH.Web.Features.Common.Models;
using TKH.Web.Features.ProductProfits.Models;

namespace TKH.Web.Features.Products.Models
{
    public class ProductProfitListViewModel
    {
        public IPagedList<ProductProfitListItemViewModel> Products { get; set; }

        public ProductProfitListFilterViewModel Filter { get; set; }

        [Display(Name = "Pazaryeri Türü")]
        public MarketplaceType MarketplaceType { get; set; }

        public MarketplaceDefaultsViewModel MarketplaceDefaults { get; set; }

        [Display(Name = "Stopaj Oranı")]
        public decimal WithholdingRate { get; set; }

        [Display(Name = "Kargo KDV Oranı")]
        public decimal ShippingVatRate { get; set; }
    }
}
