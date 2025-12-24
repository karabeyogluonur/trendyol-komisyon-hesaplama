using System.Text.Json.Serialization;

namespace TKH.Business.Integrations.Providers.Trendyol.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TrendyolOrderByDirection
    {
        ASC,
        DESC
    }
}
