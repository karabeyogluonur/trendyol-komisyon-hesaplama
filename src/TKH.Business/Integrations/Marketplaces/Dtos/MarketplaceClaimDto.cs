using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Marketplaces.Dtos
{
    public class MarketplaceClaimDto
    {
        public int MarketplaceAccountId { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string ExternalOrderNumber { get; set; } = string.Empty;
        public string ExternalShipmentPackageId { get; set; } = string.Empty;
        public DateTime ClaimDate { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public string CustomerFirstName { get; set; } = string.Empty;
        public string CustomerLastName { get; set; } = string.Empty;
        public string CargoTrackingNumber { get; set; } = string.Empty;
        public string CargoProviderName { get; set; } = string.Empty;
        public string CargoSenderNumber { get; set; } = string.Empty;
        public string CargoTrackingLink { get; set; } = string.Empty;
        public string? RejectedExternalPackageId { get; set; }
        public string? RejectedCargoTrackingNumber { get; set; }
        public string? RejectedCargoProviderName { get; set; }
        public string? RejectedCargoTrackingLink { get; set; }
        public List<MarketplaceClaimItemDto> Items { get; set; } = new();
    }

    public class MarketplaceClaimItemDto
    {
        public string ExternalId { get; set; } = string.Empty;
        public string ExternalOrderLineItemId { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal VatRate { get; set; }
        public ClaimStatus Status { get; set; }
        public ClaimReasonType ReasonType { get; set; }
        public string ReasonName { get; set; } = string.Empty;
        public string ReasonCode { get; set; } = string.Empty;
        public string CustomerNote { get; set; } = string.Empty;
        public bool IsResolved { get; set; }
        public bool IsAutoAccepted { get; set; }
        public bool IsAcceptedBySeller { get; set; }
    }
}
