using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;

namespace TKH.Entities
{
    public class ShipmentTransaction : BaseEntity, IEntity
    {
        public int MarketplaceAccountId { get; set; }
        public string MarketplaceOrderNumber { get; set; } = string.Empty;
        public string MarketplaceParcelId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Desi { get; set; }
        public virtual MarketplaceAccount MarketplaceAccount { get; set; }
    }
}
