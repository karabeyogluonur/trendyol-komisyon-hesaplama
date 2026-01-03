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

        public decimal WithholdingRate { get; set; }          // Stopaj Oranı (Örn: 0.01)
        public decimal ExportServiceFeeRate { get; set; }     // İhracat Kesinti Oranı (Örn: 0.05)

        public decimal StandardServiceFee { get; set; }       // Standart Hizmet Bedeli (Örn: 10.19)
        public decimal SameDayServiceFee { get; set; }        // Aynı Gün Hizmet Bedeli (Örn: 6.59)

        public decimal ServiceFeeVatRate { get; set; }        // Hizmet Bedeli KDV
        public decimal CommissionVatRate { get; set; }        // Komisyon KDV
        public decimal ShippingVatRate { get; set; }          // Kargo KDV
        public decimal ExportServiceFeeVatRate { get; set; }  // İhracat Bedeli KDV
    }
}
