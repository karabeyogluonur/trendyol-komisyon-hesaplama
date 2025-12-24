using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class Order : BaseEntity, IEntity
    {
        public string MarketplaceOrderNumber { get; set; } = string.Empty;
        public int MarketplaceAccountId { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Amount { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public MarketplaceAccount MarketplaceAccount { get; set; }
    }
}
