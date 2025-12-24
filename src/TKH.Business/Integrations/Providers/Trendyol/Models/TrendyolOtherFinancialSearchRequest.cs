using Refit;
using TKH.Business.Integrations.Providers.Trendyol.Enums;

namespace TKH.Business.Integrations.Providers.Trendyol.Models
{
    public class TrendyolOtherFinancialSearchRequest
    {
        [AliasAs("startDate")]
        public long StartDate { get; set; }

        [AliasAs("endDate")]
        public long EndDate { get; set; }

        [AliasAs("page")]
        public int? Page { get; set; }

        [AliasAs("size")]
        public int? Size { get; set; }

        [AliasAs("supplierId")]
        public long? SupplierId { get; set; }

        [AliasAs("transactionType")]
        public TrendyolOtherFinancialTransactionType TransactionType { get; set; }
    }
}
