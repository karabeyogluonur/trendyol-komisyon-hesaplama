using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class ClaimItem : BaseEntity, IEntity
    {
        #region Properties

        public int ClaimId { get; private set; }
        public int? ProductId { get; private set; }

        public string ExternalId { get; private set; } = string.Empty;
        public string ExternalOrderLineItemId { get; private set; } = string.Empty;

        public string Barcode { get; private set; } = string.Empty;
        public string Sku { get; private set; } = string.Empty;
        public string ProductName { get; private set; } = string.Empty;

        public decimal Price { get; private set; }
        public decimal VatRate { get; private set; }

        public ClaimStatus Status { get; private set; }

        public string CustomerNote { get; private set; } = string.Empty;
        public ClaimReasonType ReasonType { get; private set; }
        public string ReasonName { get; private set; } = string.Empty;
        public string ReasonCode { get; private set; } = string.Empty;

        public bool IsResolved { get; private set; }
        public bool IsAutoAccepted { get; private set; }
        public bool IsAcceptedBySeller { get; private set; }

        public virtual Claim Claim { get; private set; }
        public virtual Product? Product { get; private set; }

        #endregion

        #region Ctor

        protected ClaimItem()
        {
        }

        #endregion

        #region Factory

        public static ClaimItem Create(
            int claimId,
            int? productId,
            string externalId,
            string externalOrderLineItemId,
            string barcode,
            string sku,
            string productName,
            decimal price,
            decimal vatRate,
            ClaimStatus status,
            string customerNote,
            ClaimReasonType reasonType,
            string reasonName,
            string reasonCode,
            bool isResolved,
            bool isAutoAccepted,
            bool isAcceptedBySeller)
        {
            return new ClaimItem
            {
                ClaimId = claimId,
                ProductId = productId,
                ExternalId = externalId,
                ExternalOrderLineItemId = externalOrderLineItemId,
                Barcode = barcode,
                Sku = sku,
                ProductName = productName,
                Price = price,
                VatRate = vatRate,
                Status = status,
                CustomerNote = customerNote,
                ReasonType = reasonType,
                ReasonName = reasonName,
                ReasonCode = reasonCode,
                IsResolved = isResolved,
                IsAutoAccepted = isAutoAccepted,
                IsAcceptedBySeller = isAcceptedBySeller
            };
        }

        #endregion
    }
}
