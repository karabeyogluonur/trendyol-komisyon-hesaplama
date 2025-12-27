using Refit;
using TKH.Business.Integrations.Providers.Trendyol.Enums;

namespace TKH.Business.Integrations.Providers.Trendyol.Models
{
    public class TrendyolClaimSearchRequest
    {
        [AliasAs("claimIds")]
        public List<string> ClaimIds { get; set; }

        [AliasAs("claimItemStatus")]
        public TrendyolClaimItemStatus? ClaimItemStatus { get; set; }

        [AliasAs("orderNumber")]
        public string OrderNumber { get; set; }

        [AliasAs("startDate")]
        public long? StartDate { get; set; }

        [AliasAs("endDate")]
        public long? EndDate { get; set; }

        [AliasAs("page")]
        public int? Page { get; set; }

        [AliasAs("size")]
        public int? Size { get; set; }
    }
}
