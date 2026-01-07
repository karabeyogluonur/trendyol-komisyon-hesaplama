namespace TKH.Core.Common.Constants
{
    public class ApplicationDefaults
    {
        public const string DemoAccountMerchantId = "633709";
        public const string BaseUploadsPath = "uploads";
        public const int DefaultPageSize = 20;
        public const int DefaultMaxPageSize = 100;
        public const int ProductPageSize = 20;
        public const int ProductBatchSize = 1000;
        public const int OrderBatchSize = 100;
        public const int FinanceBatchSize = 500;
        public const int ReferenceBatchSize = 50;
        public const int ClaimBatchSize = 100;
        public const int MarketplaceSyncParallelism = 10;
        public const int ShippingCostAnalysisLookbackDays = 60;
        public const int ProductCommissionRateAnalysisLookbackDays = 30;
        public const decimal MinimumShippingCostThreshold = 1;
        public const int ExpenseSyncBatchSize = 100;
    }
}
