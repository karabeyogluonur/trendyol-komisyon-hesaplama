using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TKH.Integrations.Trendyol.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TrendyolOrderStatus
    {
        Created,
        Picking,
        Picked,
        Shipped,
        Delivered,
        Cancelled,
        Returned,
        Returning,
        Awaiting
    }

}


