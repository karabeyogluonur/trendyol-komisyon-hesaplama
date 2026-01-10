using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class Claim : BaseEntity, IEntity, IHasMarketplaceAccount
    {
        #region Properties

        public int MarketplaceAccountId { get; private set; }

        public string ExternalId { get; private set; } = string.Empty;
        public string ExternalOrderNumber { get; private set; } = string.Empty;
        public string ExternalShipmentPackageId { get; private set; } = string.Empty;

        public DateTime ClaimDate { get; private set; }
        public DateTime OrderDate { get; private set; }
        public DateTime LastUpdateDateTime { get; private set; }

        public string CustomerFirstName { get; private set; } = string.Empty;
        public string CustomerLastName { get; private set; } = string.Empty;

        public string CargoTrackingNumber { get; private set; } = string.Empty;
        public string CargoProviderName { get; private set; } = string.Empty;
        public string CargoSenderNumber { get; private set; } = string.Empty;
        public string CargoTrackingLink { get; private set; } = string.Empty;

        public string? RejectedExternalPackageId { get; private set; }
        public string? RejectedCargoTrackingNumber { get; private set; }
        public string? RejectedCargoProviderName { get; private set; }
        public string? RejectedCargoTrackingLink { get; private set; }

        public virtual MarketplaceAccount MarketplaceAccount { get; private set; }
        public virtual ICollection<ClaimItem> ClaimItems { get; private set; } = new List<ClaimItem>();

        #endregion

        #region Interface Implementation

        int IHasMarketplaceAccount.MarketplaceAccountId
        {
            get => MarketplaceAccountId;
            set => MarketplaceAccountId = value;
        }

        #endregion

        #region Ctor

        protected Claim()
        {
        }

        #endregion

        #region Factory

        public static Claim Create(
            int marketplaceAccountId,
            string externalId,
            string externalOrderNumber,
            string externalShipmentPackageId,
            DateTime claimDate,
            DateTime orderDate,
            string customerFirstName,
            string customerLastName,
            string cargoTrackingNumber,
            string cargoProviderName,
            string cargoSenderNumber,
            string cargoTrackingLink,
            string? rejectedExternalPackageId,
            string? rejectedCargoTrackingNumber,
            string? rejectedCargoProviderName,
            string? rejectedCargoTrackingLink)
        {
            return new Claim
            {
                MarketplaceAccountId = marketplaceAccountId,
                ExternalId = externalId,
                ExternalOrderNumber = externalOrderNumber,
                ExternalShipmentPackageId = externalShipmentPackageId,
                ClaimDate = claimDate,
                OrderDate = orderDate,
                LastUpdateDateTime = DateTime.UtcNow,
                CustomerFirstName = customerFirstName,
                CustomerLastName = customerLastName,
                CargoTrackingNumber = cargoTrackingNumber,
                CargoProviderName = cargoProviderName,
                CargoSenderNumber = cargoSenderNumber,
                CargoTrackingLink = cargoTrackingLink,
                RejectedExternalPackageId = rejectedExternalPackageId,
                RejectedCargoTrackingNumber = rejectedCargoTrackingNumber,
                RejectedCargoProviderName = rejectedCargoProviderName,
                RejectedCargoTrackingLink = rejectedCargoTrackingLink
            };
        }

        #endregion

        #region Update Methods

        public void UpdateDetails(
            string customerFirstName,
            string customerLastName,
            string cargoTrackingNumber,
            string cargoProviderName,
            string cargoSenderNumber,
            string cargoTrackingLink,
            string? rejectedExternalPackageId,
            string? rejectedCargoTrackingNumber,
            string? rejectedCargoProviderName,
            string? rejectedCargoTrackingLink,
            DateTime lastUpdateDateTime)
        {
            CustomerFirstName = customerFirstName;
            CustomerLastName = customerLastName;
            CargoTrackingNumber = cargoTrackingNumber;
            CargoProviderName = cargoProviderName;
            CargoSenderNumber = cargoSenderNumber;
            CargoTrackingLink = cargoTrackingLink;
            RejectedExternalPackageId = rejectedExternalPackageId;
            RejectedCargoTrackingNumber = rejectedCargoTrackingNumber;
            RejectedCargoProviderName = rejectedCargoProviderName;
            RejectedCargoTrackingLink = rejectedCargoTrackingLink;
            LastUpdateDateTime = lastUpdateDateTime;
        }

        public void SyncItems(IEnumerable<ClaimItem> newClaimItems)
        {
            if (ClaimItems.Any())
                ClaimItems.Clear();

            foreach (ClaimItem claimItem in newClaimItems)
                ClaimItems.Add(claimItem);
        }

        #endregion
    }
}
