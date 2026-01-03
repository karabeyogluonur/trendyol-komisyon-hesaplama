namespace TKH.Business.Integrations.Providers.Trendyol
{
    internal static class TrendyolDefaults
    {
        public const decimal FixedServiceFeeAmount = 10.19m;
        public const decimal FixedSameDayServiceFeeAmount = 6.59m;
        public const bool IsFixedServiceFeeVatIncluded = true;


        public const string BaseUrl = "https://apigw.trendyol.com";
        public const string HttpClientName = "TrendyolClient";
        public const string UserAgentSuffix = "OK";


        public const int ProductPageSize = 100;
        public const int OrderPageSize = 50;
        public const int ClaimPageSize = 50;
        public const int FinancePageSize = 500;


        public const int PermitLimit = 1;
        public const int QueueLimit = 1000;
        public static readonly TimeSpan ReplenishmentPeriod = TimeSpan.FromMilliseconds(1000);
        public const int TokensPerPeriod = 1;

        public const int OrderSyncLookbackMonths = -2;
        public const int OrderSyncDateWindowDays = 14;

        public const int ClaimSyncLookbackMonths = -2;
        public const int ClaimSyncDateWindowDays = 14;

        public const int FinanceSyncLookbackMonths = -2;
        public const int FinanceSyncDateWindowDays = 14;

        public const int TimeZoneOffsetHours = 3;
    }
}
