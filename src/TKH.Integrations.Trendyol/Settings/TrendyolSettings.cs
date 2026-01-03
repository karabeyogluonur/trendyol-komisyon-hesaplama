using TKH.Core.Entities.Abstract;

namespace TKH.Integrations.Trendyol.Settings
{
    public class TrendyolSettings : ISettings
    {
        public decimal ServiceFeeAmount { get; set; } = 10.19m;
        public decimal ServiceFeeVatRate { get; set; } = 20;
        public decimal SameDayServiceFeeAmount { get; set; } = 6.59m;
        public decimal ExportServiceFeeRate { get; set; } = 5;
        public decimal ExportServiceFeeVatRate { get; set; } = 20;
        public decimal ProductCommissionVatRate { get; set; } = 20;
        public string BaseUrl { get; set; } = "https://apigw.trendyol.com";
        public string UserAgent { get; set; } = "OK";
    }
}
