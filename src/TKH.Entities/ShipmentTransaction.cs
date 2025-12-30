using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;

namespace TKH.Entities
{
    public class ShipmentTransaction : BaseEntity, IEntity, IHasMarketplaceAccount
    {
        public int MarketplaceAccountId { get; set; }
        public string ExternalOrderNumber { get; set; } = string.Empty;
        public string ExternalParcelId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Deci { get; set; }
        public virtual MarketplaceAccount MarketplaceAccount { get; set; }
    }
}
