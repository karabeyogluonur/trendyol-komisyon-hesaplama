using System.Text.Json.Serialization;

namespace TKH.Business.Integrations.Providers.Trendyol.Models
{
    public class TrendyolResponseGetProducts
    {
        [JsonPropertyName("content")]
        public List<TrendyolProductContent> Content { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("totalElements")]
        public long TotalElements { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }
    }
}
