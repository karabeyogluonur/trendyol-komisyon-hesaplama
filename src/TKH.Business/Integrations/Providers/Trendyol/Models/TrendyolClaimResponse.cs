using System.Text.Json.Serialization;
using TKH.Business.Integrations.Providers.Trendyol.Enums;

namespace TKH.Business.Integrations.Providers.Trendyol.Models
{
    public class TrendyolClaimResponse
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
        public List<TrendyolClaimContent> Content { get; set; }
    }

    public class TrendyolClaimContent
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("claimId")]
        public string ClaimId { get; set; }

        [JsonPropertyName("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonPropertyName("orderDate")]
        public long OrderDate { get; set; }

        [JsonPropertyName("claimDate")]
        public long ClaimDate { get; set; }

        [JsonPropertyName("customerFirstName")]
        public string CustomerFirstName { get; set; }

        [JsonPropertyName("customerLastName")]
        public string CustomerLastName { get; set; }

        [JsonPropertyName("cargoTrackingNumber")]
        public long? CargoTrackingNumber { get; set; }

        [JsonPropertyName("cargoTrackingLink")]
        public string CargoTrackingLink { get; set; }

        [JsonPropertyName("cargoProviderName")]
        public string CargoProviderName { get; set; }

        [JsonPropertyName("cargoSenderNumber")]
        public string CargoSenderNumber { get; set; }

        [JsonPropertyName("orderShipmentPackageId")]
        public long OrderShipmentPackageId { get; set; }

        [JsonPropertyName("lastModifiedDate")]
        public long LastModifiedDate { get; set; }

        [JsonPropertyName("rejectedpackageinfo")]
        public TrendyolPackageInfo RejectedPackageInfo { get; set; }

        [JsonPropertyName("replacementOutboundpackageinfo")]
        public TrendyolPackageInfo ReplacementPackageInfo { get; set; }

        [JsonPropertyName("items")]
        public List<TrendyolClaimLineItem> Items { get; set; }
    }

    public class TrendyolPackageInfo
    {
        [JsonPropertyName("packageid")]
        public long PackageId { get; set; }

        [JsonPropertyName("cargoTrackingNumber")]
        public long? CargoTrackingNumber { get; set; }

        [JsonPropertyName("cargoProviderName")]
        public string CargoProviderName { get; set; }

        [JsonPropertyName("cargoSenderNumber")]
        public string CargoSenderNumber { get; set; }

        [JsonPropertyName("cargoTrackingLink")]
        public string CargoTrackingLink { get; set; }

        [JsonPropertyName("items")]
        public List<string> Items { get; set; }
    }

    public class TrendyolClaimLineItem
    {
        [JsonPropertyName("orderLine")]
        public TrendyolClaimOrderLine OrderLine { get; set; }

        [JsonPropertyName("claimItems")]
        public List<TrendyolClaimItemDetail> ClaimItems { get; set; }
    }

    public class TrendyolClaimOrderLine
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; }

        [JsonPropertyName("barcode")]
        public string Barcode { get; set; }

        [JsonPropertyName("merchantSku")]
        public string MerchantSku { get; set; }

        [JsonPropertyName("productColor")]
        public string ProductColor { get; set; }

        [JsonPropertyName("productSize")]
        public string ProductSize { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("vatBaseAmount")]
        public decimal VatBaseAmount { get; set; }

        [JsonPropertyName("vatRate")]
        public decimal VatRate { get; set; }

        [JsonPropertyName("productCategory")]
        public string ProductCategory { get; set; }
    }

    public class TrendyolClaimItemDetail
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("orderLineItemId")]
        public long OrderLineItemId { get; set; }

        [JsonPropertyName("customerClaimItemReason")]
        public TrendyolClaimReasonObj CustomerClaimItemReason { get; set; }

        [JsonPropertyName("trendyolClaimItemReason")]
        public TrendyolClaimReasonObj TrendyolClaimItemReason { get; set; }

        [JsonPropertyName("claimItemStatus")]
        public TrendyolClaimStatusObj ClaimItemStatus { get; set; }

        [JsonPropertyName("customerNote")]
        public string CustomerNote { get; set; }

        [JsonPropertyName("note")]
        public string Note { get; set; }

        [JsonPropertyName("resolved")]
        public bool? Resolved { get; set; }

        [JsonPropertyName("autoAccepted")]
        public bool? AutoAccepted { get; set; }

        [JsonPropertyName("acceptedBySeller")]
        public bool? AcceptedBySeller { get; set; }
    }

    public class TrendyolClaimReasonObj
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("externalReasonId")]
        public int? ExternalReasonId { get; set; }

        [JsonIgnore]
        public TrendyolReturnReason ReasonEnum
        {
            get
            {
                if (Enum.IsDefined(typeof(TrendyolReturnReason), Id))
                {
                    return (TrendyolReturnReason)Id;
                }
                return TrendyolReturnReason.None;
            }
        }
    }

    public class TrendyolClaimStatusObj
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
