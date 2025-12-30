using System.Text.Json.Serialization;

namespace TKH.Business.Integrations.Providers.Trendyol.Models
{
    public class TrendyolCategoryResponse
    {
        [JsonPropertyName("categories")]
        public List<TrendyolCategoryContent> Categories { get; set; } = new();
    }

    public class TrendyolCategoryContent
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("parentId")]
        public int? ParentId { get; set; }

        [JsonPropertyName("subCategories")]
        public List<TrendyolCategoryContent> SubCategories { get; set; } = new();
    }
}
