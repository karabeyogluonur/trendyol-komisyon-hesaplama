namespace TKH.Business.Integrations.Providers.Trendyol
{
    internal static class TrendyolDefaults
    {
        public const string BaseUrl = "https://apigw.trendyol.com";
        public const string HttpClientName = "TrendyolClient";
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 100;
        public const int BatchSize = 1000;
        public const string UserAgentSuffix = "OK";
    }
}
