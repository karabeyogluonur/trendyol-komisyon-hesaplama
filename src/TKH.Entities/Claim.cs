using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class Claim : BaseEntity, IEntity
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
        public virtual MarketplaceAccount MarketplaceAccount { get; set; }
        public virtual ICollection<ClaimItem> ClaimItems { get; set; } = new List<ClaimItem>();
    }
}
