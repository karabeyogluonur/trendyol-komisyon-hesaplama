using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TKH.Business.Integrations.Providers.Trendyol.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TrendyolDateQueryType
    {
        [EnumMember(Value = "CREATED_DATE")]
        CreatedDate,

        [EnumMember(Value = "LAST_MODIFIED_DATE")]
        LastModifiedDate
    }
}
