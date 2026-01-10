using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class ProductPrice : BaseEntity, IEntity
    {
        #region Properties

        public int ProductId { get; private set; }
        public virtual Product Product { get; private set; }

        public ProductPriceType Type { get; private set; }
        public decimal Amount { get; private set; }
        public bool IsVatIncluded { get; private set; }

        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        #endregion

        #region Ctor

        protected ProductPrice()
        {
        }

        #endregion

        #region Factory

        public static ProductPrice Create(int productId, ProductPriceType productPriceType, decimal amount, bool isVatIncluded = true)
        {
            return new ProductPrice
            {
                ProductId = productId,
                Type = productPriceType,
                Amount = amount,
                IsVatIncluded = isVatIncluded,
                StartDate = DateTime.UtcNow,
                EndDate = null
            };
        }

        #endregion

        #region Behavior

        public bool IsActive()
        {
            return EndDate == null;
        }

        public bool ShouldUpdate(decimal newAmount)
        {
            return Amount != newAmount;
        }

        public void MarkAsExpired()
        {
            if (EndDate.HasValue)
                return;

            EndDate = DateTime.UtcNow;
        }

        #endregion
    }
}
