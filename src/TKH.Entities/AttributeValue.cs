using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;

namespace TKH.Entities
{
    public class AttributeValue : BaseEntity, IEntity
    {
        #region Properties

        public int CategoryAttributeId { get; private set; }

        public string ExternalId { get; private set; } = string.Empty;
        public string Value { get; private set; } = string.Empty;

        public virtual CategoryAttribute Attribute { get; private set; }
        public virtual ICollection<ProductAttribute> ProductAttributes { get; private set; } = new List<ProductAttribute>();

        #endregion

        #region Ctor

        protected AttributeValue()
        {
        }

        #endregion

        #region Factory

        public static AttributeValue Create(int categoryAttributeId, string externalId, string value)
        {
            return new AttributeValue
            {
                CategoryAttributeId = categoryAttributeId,
                ExternalId = externalId,
                Value = value
            };
        }

        #endregion

        #region Behavior

        public void UpdateValue(string value)
        {
            if (Value == value)
                return;

            Value = value;
        }

        #endregion
    }
}
