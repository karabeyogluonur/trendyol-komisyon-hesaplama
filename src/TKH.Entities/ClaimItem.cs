using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class ClaimItem : BaseEntity, IEntity
    {
        public int ClaimId { get; set; }
        public int? ProductId { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string ExternalOrderLineItemId { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal VatRate { get; set; }
        public ClaimStatus Status { get; set; }
        public string CustomerNote { get; set; } = string.Empty;
        public ClaimReasonType ReasonType { get; set; }
        public string ReasonName { get; set; } = string.Empty;
        public string ReasonCode { get; set; } = string.Empty;
        public bool IsResolved { get; set; }
        public bool IsAutoAccepted { get; set; }
        public bool IsAcceptedBySeller { get; set; }
        public virtual Claim Claim { get; set; }
        public virtual Product? Product { get; set; }
    }
}
