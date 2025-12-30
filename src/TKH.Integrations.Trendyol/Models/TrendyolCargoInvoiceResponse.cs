using System.Text.Json.Serialization;

namespace TKH.Business.Integrations.Providers.Trendyol.Models
{
    public class TrendyolCargoInvoiceResponse
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("totalElements")]
        public int TotalElements { get; set; }

        [JsonPropertyName("content")]
        public List<TrendyolCargoInvoiceContent> Content { get; set; } = new();
    }
    public class TrendyolCargoInvoiceContent
    {
        [JsonPropertyName("shipmentPackageType")]
        public string ShipmentPackageType { get; set; } = string.Empty;

        [JsonPropertyName("parcelUniqueId")]
        public long ParcelUniqueId { get; set; }

        [JsonPropertyName("orderNumber")]
        public string OrderNumber { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("desi")]
        public int Desi { get; set; }
    }
}
