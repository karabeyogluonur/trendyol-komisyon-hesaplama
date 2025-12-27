namespace TKH.Business.Integrations.Providers.Trendyol
{
    internal static class TrendyolDefaults
    {
        public const decimal FixedServiceFeeAmount = 12.47m;
        public const bool IsFixedServiceFeeVatIncluded = true;

        public const string BaseUrl = "https://apigw.trendyol.com";
        public const string HttpClientName = "TrendyolClient";
        public const string UserAgentSuffix = "OK";

        public const int ProductPageSize = 100;
        public const int OrderPageSize = 50;
        public const int ClaimPageSize = 50;

        public const int ApiRateLimitDelayMs = 200;

        public const int OrderSyncLookbackMonths = -2;
        public const int OrderSyncDateWindowDays = 14;

        public const int ClaimSyncLookbackMonths = -2;
        public const int ClaimSyncDateWindowDays = 14;
        public const int TimeZoneOffsetHours = 3;

        public const int FinancePageSize = 500;
        public const int FinanceSyncLookbackMonths = -2;
        public const int FinanceSyncDateWindowDays = 14;
    }
}
