using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;

namespace TKH.Entities
{
    public class CategoryAttribute : BaseEntity, IEntity
    {
        #region Properties

        public int CategoryId { get; private set; }

        public string ExternalId { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public bool IsVariant { get; private set; }

        public virtual Category Category { get; private set; }
        public virtual ICollection<AttributeValue> Values { get; private set; } = new List<AttributeValue>();
        public virtual ICollection<ProductAttribute> ProductAttributes { get; private set; } = new List<ProductAttribute>();

        #endregion

        #region Ctor

        protected CategoryAttribute()
        {
        }

        #endregion

        #region Factory

        public static CategoryAttribute Create(int categoryId, string externalId, string name, bool isVariant)
        {
            return new CategoryAttribute
            {
                CategoryId = categoryId,
                ExternalId = externalId,
                Name = name,
                IsVariant = isVariant
            };
        }

        #endregion

        #region Behavior

        public void UpdateDetails(string name, bool isVariant)
        {
            Name = name;
            IsVariant = isVariant;
        }

        public void AddOrUpdateValue(string externalId, string value)
        {
            AttributeValue? existingAttributeValueEntity = Values.FirstOrDefault(val => val.ExternalId == externalId);

            if (existingAttributeValueEntity is not null)
                existingAttributeValueEntity.UpdateValue(value);
            else
            {
                AttributeValue newAttributeValueEntity = AttributeValue.Create(
                    this.Id,
                    externalId,
                    value
                );

                Values.Add(newAttributeValueEntity);
            }
        }

        #endregion
    }
}
