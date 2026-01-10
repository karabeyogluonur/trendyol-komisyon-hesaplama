using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;

namespace TKH.Entities
{
    public class ShipmentTransaction : BaseEntity, IEntity, IHasMarketplaceAccount
    {
        #region Properties

        public int MarketplaceAccountId { get; private set; }

        public string ExternalOrderNumber { get; private set; } = string.Empty;
        public string ExternalParcelId { get; private set; } = string.Empty;

        public decimal Amount { get; private set; }
        public int Deci { get; private set; }

        public virtual MarketplaceAccount MarketplaceAccount { get; private set; }

        #endregion

        #region Interface Implementation

        int IHasMarketplaceAccount.MarketplaceAccountId
        {
            get => MarketplaceAccountId;
            set => MarketplaceAccountId = value;
        }

        #endregion

        #region Ctor

        protected ShipmentTransaction()
        {
        }

        #endregion

        #region Factory

        public static ShipmentTransaction Create(
            int marketplaceAccountId,
            string externalOrderNumber,
            string externalParcelId,
            decimal amount,
            int deci)
        {
            return new ShipmentTransaction
            {
                MarketplaceAccountId = marketplaceAccountId,
                ExternalOrderNumber = externalOrderNumber,
                ExternalParcelId = externalParcelId,
                Amount = amount,
                Deci = deci
            };
        }

        #endregion
    }
}
