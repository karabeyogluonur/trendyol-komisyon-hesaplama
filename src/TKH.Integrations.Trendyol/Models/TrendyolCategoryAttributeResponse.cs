using System.Text.Json.Serialization;

namespace TKH.Business.Integrations.Providers.Trendyol.Models
{
    public class TrendyolCategoryAttributeResponse
    {
        [JsonPropertyName("id")]
        public int CategoryId { get; set; }

        [JsonPropertyName("name")]
        public string CategoryName { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("categoryAttributes")]
        public List<TrendyolCategoryAttributeContent> CategoryAttributes { get; set; } = new();
    }

    public class TrendyolCategoryAttributeContent
    {
        [JsonPropertyName("allowCustom")]
        public bool AllowCustom { get; set; }

        [JsonPropertyName("required")]
        public bool Required { get; set; }

        [JsonPropertyName("varianter")]
        public bool Varianter { get; set; }

        [JsonPropertyName("slicer")]
        public bool Slicer { get; set; }

        [JsonPropertyName("attribute")]
        public TrendyolAttributeDefinition Attribute { get; set; }

        [JsonPropertyName("attributeValues")]
        public List<TrendyolAttributeValue> AttributeValues { get; set; } = new();
    }

    public class TrendyolAttributeDefinition
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class TrendyolAttributeValue
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
