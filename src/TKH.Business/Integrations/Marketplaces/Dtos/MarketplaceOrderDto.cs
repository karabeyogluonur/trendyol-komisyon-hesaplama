using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Marketplaces.Dtos
{
    public class MarketplaceOrderDto
    {
        public string ExternalOrderNumber { get; set; } = string.Empty;
        public string ExternalShipmentId { get; set; } = string.Empty;
        public int MarketplaceAccountId { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal PlatformCoveredDiscount { get; set; }
        public string CurrencyCode { get; set; } = "TRY";
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public string CargoTrackingNumber { get; set; } = string.Empty;
        public string CargoProviderName { get; set; } = string.Empty;
        public double Deci { get; set; }
        public bool IsShipmentPaidBySeller { get; set; }
        public bool IsMicroExport { get; set; }
        public List<MarketplaceOrderItemDto> Items { get; set; } = new();
    }

    public class MarketplaceOrderItemDto
    {
        public string Sku { get; set; } = string.Empty;
        public string ExternalProductCode { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VatRate { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal PlatformCoveredDiscount { get; set; }
        public decimal SellerCoveredDiscount { get; set; }
        public OrderItemStatus OrderItemStatus { get; set; }
    }
}
