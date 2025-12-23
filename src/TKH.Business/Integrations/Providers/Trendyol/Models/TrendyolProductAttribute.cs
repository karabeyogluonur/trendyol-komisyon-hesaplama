using System.Text.Json.Serialization;

namespace TKH.Business.Integrations.Providers.Trendyol.Models
{
    public class TrendyolProductAttribute
    {
        [JsonPropertyName("attributeId")]
        public long AttributeId { get; set; }

        [JsonPropertyName("attributeValueId")]
        public long? AttributeValueId { get; set; }

        [JsonPropertyName("customAttributeValue")]
        public string CustomAttributeValue { get; set; }
    }
}
