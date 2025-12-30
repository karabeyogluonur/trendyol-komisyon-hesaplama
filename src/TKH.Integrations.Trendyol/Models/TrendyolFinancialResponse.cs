using System.Text.Json.Serialization;

namespace TKH.Business.Integrations.Providers.Trendyol.Models
{
    public class TrendyolFinancialResponse
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
        public List<TrendyolFinancialContent> Content { get; set; } = new();
    }

    public class TrendyolFinancialContent
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("transactionDate")]
        public long TransactionDate { get; set; }

        [JsonPropertyName("barcode")]
        public string? Barcode { get; set; }

        [JsonPropertyName("transactionType")]
        public string TransactionType { get; set; }

        [JsonPropertyName("receiptId")]
        public object? ReceiptId { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("debt")]
        public decimal Debt { get; set; }

        [JsonPropertyName("credit")]
        public decimal Credit { get; set; }

        [JsonPropertyName("paymentPeriod")]
        public int? PaymentPeriod { get; set; }

        [JsonPropertyName("commissionRate")]
        public decimal? CommissionRate { get; set; }

        [JsonPropertyName("commissionAmount")]
        public decimal? CommissionAmount { get; set; }

        [JsonPropertyName("sellerRevenue")]
        public decimal? SellerRevenue { get; set; }

        [JsonPropertyName("orderNumber")]
        public string? OrderNumber { get; set; }

        [JsonPropertyName("paymentOrderId")]
        public long? PaymentOrderId { get; set; }

        [JsonPropertyName("paymentDate")]
        public long? PaymentDate { get; set; }

        [JsonPropertyName("sellerId")]
        public long? SellerId { get; set; }

        [JsonPropertyName("shipmentPackageId")]
        public long? ShipmentPackageId { get; set; }
    }
}
