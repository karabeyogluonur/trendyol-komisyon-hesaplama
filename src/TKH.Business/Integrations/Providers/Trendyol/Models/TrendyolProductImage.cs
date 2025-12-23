using System.Text.Json.Serialization;

namespace TKH.Business.Integrations.Providers.Trendyol.Models
{
    public class TrendyolProductImage
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
