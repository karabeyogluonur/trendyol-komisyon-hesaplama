using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;

namespace TKH.Entities
{
    public class ProductAttribute : BaseEntity, IEntity
    {
        #region Properties

        public int ProductId { get; private set; }
        public int CategoryAttributeId { get; private set; }

        public int? AttributeValueId { get; private set; }
        public string? CustomValue { get; private set; }

        public virtual Product Product { get; private set; }
        public virtual CategoryAttribute Attribute { get; private set; }
        public virtual AttributeValue? Value { get; private set; }

        #endregion

        #region Ctor

        protected ProductAttribute()
        {
        }

        #endregion

        #region Factory

        public static ProductAttribute Create(int productId, int categoryAttributeId, int? attributeValueId, string? customValue)
        {
            return new ProductAttribute
            {
                ProductId = productId,
                CategoryAttributeId = categoryAttributeId,
                AttributeValueId = attributeValueId,
                CustomValue = customValue
            };
        }

        #endregion

        #region Behavior

        public void UpdateValue(int? attributeValueId, string? customValue)
        {
            if (AttributeValueId == attributeValueId && CustomValue == customValue)
                return;

            AttributeValueId = attributeValueId;
            CustomValue = customValue;
        }

        #endregion
    }
}
