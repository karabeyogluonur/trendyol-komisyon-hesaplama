using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class Order : BaseEntity, IEntity
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
        public DateTime LastUpdateDateTime { get; set; }
        public string CargoTrackingNumber { get; set; } = string.Empty;
        public string CargoProviderName { get; set; } = string.Empty;
        public double Deci { get; set; }
        public bool IsShipmentPaidBySeller { get; set; }
        public bool IsMicroExport { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual MarketplaceAccount MarketplaceAccount { get; set; }
    }
}
