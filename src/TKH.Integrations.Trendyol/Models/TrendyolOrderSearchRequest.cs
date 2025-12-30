using Refit;
using TKH.Integrations.Trendyol.Enums;

namespace TKH.Business.Integrations.Providers.Trendyol.Models
{
    public class TrendyolOrderSearchRequest
    {
        [AliasAs("startDate")]
        public long? StartDate { get; set; }

        [AliasAs("endDate")]
        public long? EndDate { get; set; }

        [AliasAs("page")]
        public int? Page { get; set; }

        [AliasAs("size")]
        public int? Size { get; set; }

        [AliasAs("orderNumber")]
        public string OrderNumber { get; set; }

        [AliasAs("status")]
        public TrendyolOrderStatus? Status { get; set; }

        [AliasAs("orderByField")]
        public TrendyolOrderByField OrderByField { get; set; }

        [AliasAs("orderByDirection")]
        public TrendyolOrderByDirection? OrderByDirection { get; set; }

        [AliasAs("shipmentPackageIds")]
        public List<long> ShipmentPackageIds { get; set; }
    }
}
