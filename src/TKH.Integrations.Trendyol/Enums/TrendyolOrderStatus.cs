using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TKH.Integrations.Trendyol.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TrendyolOrderStatus
    {
        Unknown = 0,

        Awaiting,
        Created,
        Picking,
        Invoiced,
        Shipped,
        AtCollectionPoint,
        UnPacked,
        Delivered,
        UnDelivered,
        Cancelled,
        Returned
    }


}


