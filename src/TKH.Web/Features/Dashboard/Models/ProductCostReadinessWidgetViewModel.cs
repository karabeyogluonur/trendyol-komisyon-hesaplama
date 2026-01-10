using TKH.Business.Features.Products.Enums;

namespace TKH.Web.Features.Dashboard.Models
{
    public class ProductCostReadinessWidgetViewModel
    {
        public int MissingPurchasePriceCount { get; set; }
        public int MissingShippingCostCount { get; set; }
        public int MissingCommissionCount { get; set; }
        public int ReadyForAnalysisCount { get; set; }
        public int TotalProductCount { get; set; }

        public int ReadyPercent => CalculatePercent(ReadyForAnalysisCount);
        public int MissingPricePercent => CalculatePercent(MissingPurchasePriceCount);
        public int MissingShippingPercent => CalculatePercent(MissingShippingCostCount);
        public int MissingCommissionPercent => CalculatePercent(MissingCommissionCount);

        private int CalculatePercent(int count)
        {
            if (TotalProductCount <= 0) return 0;
            return (int)Math.Round((double)count / TotalProductCount * 100);
        }

        public string HealthStatusColor => ReadyPercent switch
        {
            >= 90 => "success",
            >= 60 => "warning",
            _ => "danger"
        };
    }
}
