using System.Text.Json.Serialization;

namespace TKH.Business.Integrations.Providers.Trendyol.Models
{
    public class TrendyolProductResponse
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
        public List<TrendyolProductContent> Content { get; set; } = new();
    }

    public class TrendyolProductContent
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("barcode")]
        public string Barcode { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("productMainId")]
        public string ProductMainId { get; set; }

        [JsonPropertyName("productCode")]
        public long ProductCode { get; set; }

        [JsonPropertyName("brandId")]
        public int BrandId { get; set; }

        [JsonPropertyName("brand")]
        public string BrandName { get; set; }

        [JsonPropertyName("pimCategoryId")]
        public int? PimCategoryId { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("stockCode")]
        public string StockCode { get; set; }

        [JsonPropertyName("dimensionalWeight")]
        public decimal? DimensionalWeight { get; set; }

        [JsonPropertyName("stockUnitType")]
        public string StockUnitType { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("currencyType")]
        public string CurrencyType { get; set; } = "TRY";

        [JsonPropertyName("listPrice")]
        public decimal ListPrice { get; set; }

        [JsonPropertyName("salePrice")]
        public decimal SalePrice { get; set; }

        [JsonPropertyName("vatRate")]
        public int VatRate { get; set; }

        [JsonPropertyName("approved")]
        public bool Approved { get; set; }

        [JsonPropertyName("archived")]
        public bool Archived { get; set; }

        [JsonPropertyName("onSale")]
        public bool OnSale { get; set; }

        [JsonPropertyName("locked")]
        public bool Locked { get; set; }

        [JsonPropertyName("blacklisted")]
        public bool Blacklisted { get; set; }

        [JsonPropertyName("createDateTime")]
        public long? CreateDateTime { get; set; }

        [JsonPropertyName("lastUpdateDate")]
        public long? LastUpdateDate { get; set; }

        [JsonPropertyName("productUrl")]
        public string ProductUrl { get; set; }

        [JsonPropertyName("images")]
        public List<TrendyolProductImage> Images { get; set; }

        [JsonPropertyName("attributes")]
        public List<TrendyolProductAttribute> Attributes { get; set; }

        [JsonPropertyName("deliveryOption")]
        public TrendyolDeliveryOption DeliveryOption { get; set; }
    }

    public class TrendyolProductImage
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class TrendyolProductAttribute
    {
        [JsonPropertyName("attributeId")]
        public int AttributeId { get; set; }

        [JsonPropertyName("attributeName")]
        public string AttributeName { get; set; }

        [JsonPropertyName("attributeValueId")]
        public int? AttributeValueId { get; set; }

        [JsonPropertyName("attributeValue")]
        public string AttributeValue { get; set; }
    }

    public class TrendyolDeliveryOption
    {
        [JsonPropertyName("deliveryDuration")]
        public int? DeliveryDuration { get; set; }

        [JsonPropertyName("fastDeliveryType")]
        public string FastDeliveryType { get; set; }
    }
}
