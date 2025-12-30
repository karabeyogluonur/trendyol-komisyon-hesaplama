using System.Text.Json.Serialization;

namespace TKH.Integrations.Trendyol.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TrendyolOrderByDirection
    {
        ASC,
        DESC
    }
}
