using System.Text.Json.Serialization;
using TKH.Integrations.Trendyol.Enums;

namespace TKH.Business.Integrations.Providers.Trendyol.Models
{
    public class TrendyolOrderResponse
    {
        [JsonPropertyName("totalElements")]
        public int TotalElements { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("content")]
        public List<TrendyolOrderContent> Content { get; set; }
    }

    public class TrendyolOrderContent
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonPropertyName("shipmentPackageId")]
        public long ShipmentPackageId { get; set; }

        [JsonPropertyName("status")]
        public TrendyolOrderStatus Status { get; set; }

        [JsonPropertyName("shipmentPackageStatus")]
        public string ShipmentPackageStatus { get; set; }

        [JsonPropertyName("orderDate")]
        public long OrderDate { get; set; }

        [JsonPropertyName("packageTyDiscount")]
        public decimal? PackageTyDiscount { get; set; }

        [JsonPropertyName("currencyCode")]
        public string? CurrencyCode { get; set; }

        [JsonPropertyName("deliveryType")]
        public string DeliveryType { get; set; }

        [JsonPropertyName("totalPrice")]
        public decimal TotalPrice { get; set; }

        [JsonPropertyName("grossAmount")]
        public decimal GrossAmount { get; set; }

        [JsonPropertyName("totalDiscount")]
        public decimal TotalDiscount { get; set; }

        [JsonPropertyName("cargoTrackingNumber")]
        public long? CargoTrackingNumber { get; set; }

        [JsonPropertyName("cargoTrackingLink")]
        public string CargoTrackingLink { get; set; }

        [JsonPropertyName("cargoProviderName")]
        public string CargoProviderName { get; set; }

        [JsonPropertyName("cargoDeci")]
        public double CargoDeci { get; set; }

        [JsonPropertyName("whoPays")]
        public TrendyolWhoPays? WhoPays { get; set; }

        [JsonPropertyName("warehouseId")]
        public long? WarehouseId { get; set; }

        [JsonPropertyName("micro")]
        public bool Micro { get; set; }

        [JsonPropertyName("etgbNo")]
        public string EtgbNo { get; set; }

        [JsonPropertyName("hsCode")]
        public string HsCode { get; set; }

        [JsonPropertyName("lines")]
        public List<TrendyolOrderLine> Lines { get; set; }

        [JsonPropertyName("shipmentAddress")]
        public TrendyolAddress ShipmentAddress { get; set; }

        [JsonPropertyName("invoiceAddress")]
        public TrendyolAddress InvoiceAddress { get; set; }

        [JsonPropertyName("packageHistories")]
        public List<TrendyolPackageHistory> PackageHistories { get; set; }
    }

    public class TrendyolOrderLine
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("productCode")]
        public long ProductCode { get; set; }

        [JsonPropertyName("merchantSku")]
        public string MerchantSku { get; set; }

        [JsonPropertyName("barcode")]
        public string Barcode { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("vatRate")]
        public decimal VatRate { get; set; }

        [JsonPropertyName("commission")]
        public decimal? Commission { get; set; }

        [JsonPropertyName("lineTyDiscount")]
        public decimal LineTyDiscount { get; set; }

        [JsonPropertyName("lineSellerDiscount")]
        public decimal LineSellerDiscount { get; set; }

        [JsonPropertyName("orderLineItemStatusName")]
        public string OrderLineItemStatusName { get; set; }
    }

    public class TrendyolAddress
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [JsonPropertyName("fullAddress")]
        public string FullAddress { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("district")]
        public string District { get; set; }

        [JsonPropertyName("neighborhood")]
        public string Neighborhood { get; set; }

        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; }

        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("taxNumber")]
        public string TaxNumber { get; set; }

        [JsonPropertyName("taxOffice")]
        public string TaxOffice { get; set; }
    }
    public class TrendyolPackageHistory
    {
        [JsonPropertyName("createdDate")]
        public long CreatedDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
