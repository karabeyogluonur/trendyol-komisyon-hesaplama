using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class OrderItem : BaseEntity, IEntity
    {
        #region Properties

        public int OrderId { get; private set; }
        public int? ProductId { get; private set; }

        public string Barcode { get; private set; } = string.Empty;
        public string Sku { get; private set; } = string.Empty;

        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        public decimal VatRate { get; private set; }
        public decimal CommissionRate { get; private set; }

        public decimal PlatformCoveredDiscount { get; private set; }
        public decimal SellerCoveredDiscount { get; private set; }

        public OrderItemStatus OrderItemStatus { get; private set; }

        public virtual Order Order { get; private set; }
        public virtual Product? Product { get; private set; }

        #endregion

        #region Ctor

        protected OrderItem()
        {
        }

        #endregion

        #region Factory

        public static OrderItem Create(
            int? productId,
            string barcode,
            string sku,
            int quantity,
            decimal unitPrice,
            decimal vatRate,
            decimal commissionRate,
            decimal platformCoveredDiscount,
            decimal sellerCoveredDiscount,
            OrderItemStatus orderItemStatus)
        {
            return new OrderItem
            {
                ProductId = productId,
                Barcode = barcode,
                Sku = sku,
                Quantity = quantity,
                UnitPrice = unitPrice,
                VatRate = vatRate,
                CommissionRate = commissionRate,
                PlatformCoveredDiscount = platformCoveredDiscount,
                SellerCoveredDiscount = sellerCoveredDiscount,
                OrderItemStatus = orderItemStatus
            };
        }

        #endregion
    }
}
