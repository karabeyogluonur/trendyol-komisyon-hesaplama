using Refit;
using TKH.Business.Integrations.Providers.Trendyol.Enums;

namespace TKH.Business.Integrations.Providers.Trendyol.Models
{
    public class TrendyolProductSearchRequest
    {
        [AliasAs("approved")]
        public bool? Approved { get; set; }

        [AliasAs("barcode")]
        public string? Barcode { get; set; }

        [AliasAs("startDate")]
        public long? StartDate { get; set; }

        [AliasAs("endDate")]
        public long? EndDate { get; set; }

        [AliasAs("page")]
        public int? Page { get; set; }

        [AliasAs("size")]
        public int? Size { get; set; }

        [AliasAs("dateQueryType")]
        public TrendyolDateQueryType? DateQueryType { get; set; }

        [AliasAs("supplierId")]
        public long? SupplierId { get; set; }

        [AliasAs("stockCode")]
        public string? StockCode { get; set; }

        [AliasAs("archived")]
        public bool? Archived { get; set; }

        [AliasAs("productMainId")]
        public string? ProductMainId { get; set; }

        [AliasAs("onSale")]
        public bool? OnSale { get; set; }

        [AliasAs("rejected")]
        public bool? Rejected { get; set; }

        [AliasAs("blacklisted")]
        public bool? Blacklisted { get; set; }

        [AliasAs("brandIds")]
        public List<int>? BrandIds { get; set; }
    }
}
