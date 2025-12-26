using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Dtos
{
    public class MarketplaceOrderDto
    {
        public string MarketplaceOrderNumber { get; set; } = string.Empty;
        public string MarketplaceShipmentId { get; set; } = string.Empty;
        public int MarketplaceAccountId { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal PlatformCoveredDiscount { get; set; }
        public string CurrencyCode { get; set; } = "TRY";
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public string CargoTrackingNumber { get; set; } = string.Empty;
        public string CargoProviderName { get; set; } = string.Empty;
        public double CargoDeci { get; set; }
        public bool IsShipmentPaidBySeller { get; set; }
        public bool IsMicroExport { get; set; }

        public List<MarketplaceOrderItemDto> Items { get; set; } = new();
    }

    public class MarketplaceOrderItemDto
    {
        public string MarketplaceSku { get; set; } = string.Empty;
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
