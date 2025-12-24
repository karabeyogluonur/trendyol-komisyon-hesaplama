using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class OrderItem : BaseEntity, IEntity
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal MarketplaceDiscount { get; set; }
        public decimal SellerDiscount { get; set; }
        public OrderItemStatus OrderItemStatus { get; set; }
        public virtual Product Product { get; set; }
        public virtual Order Order { get; set; }
    }
}
