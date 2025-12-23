using System.Text.Json.Serialization;

namespace TKH.Business.Integrations.Providers.Trendyol.Models
{
    public class TrendyolProductDeliveryOption
    {
        [JsonPropertyName("deliveryDuration")]
        public int DeliveryDuration { get; set; }

        [JsonPropertyName("fastDeliveryType")]
        public string FastDeliveryType { get; set; }
    }
}
