namespace TKH.Business.Features.Analytics.Dtos
{
    public class ProductCostReadinessReportDto
    {
        public int MissingPurchasePriceCount { get; set; }
        public int MissingShippingCostCount { get; set; }
        public int ReadyForAnalysisCount { get; set; }
        public int MissingCommissionCount { get; set; }
        public int TotalProductCount { get; set; }
    }
}
