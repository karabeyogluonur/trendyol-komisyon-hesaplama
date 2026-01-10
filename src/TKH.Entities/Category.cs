using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class Category : BaseEntity, IEntity
    {
        #region Properties

        public MarketplaceType MarketplaceType { get; private set; }

        public string ExternalId { get; private set; } = string.Empty;
        public string? ParentExternalId { get; private set; }
        public string Name { get; private set; } = string.Empty;

        public bool IsLeaf { get; private set; }
        public decimal? DefaultCommissionRate { get; private set; }

        public virtual ICollection<CategoryAttribute> Attributes { get; private set; } = new List<CategoryAttribute>();
        public virtual ICollection<Product> Products { get; private set; } = new List<Product>();

        #endregion

        #region Ctor

        protected Category()
        {
        }

        #endregion

        #region Factory

        public static Category Create(
            MarketplaceType marketplaceType,
            string externalId,
            string? parentExternalId,
            string name,
            bool isLeaf)
        {
            return new Category
            {
                MarketplaceType = marketplaceType,
                ExternalId = externalId,
                ParentExternalId = parentExternalId,
                Name = name,
                IsLeaf = isLeaf
            };
        }

        #endregion

        #region Behavior

        public void UpdateDetails(string? parentExternalId, string name, bool isLeaf)
        {
            ParentExternalId = parentExternalId;
            Name = name;
            IsLeaf = isLeaf;
        }

        public CategoryAttribute AddOrUpdateAttribute(string externalId, string name, bool isVariant)
        {
            CategoryAttribute? existingCategoryAttributeEntity = Attributes.FirstOrDefault(attribute => attribute.ExternalId == externalId);

            if (existingCategoryAttributeEntity is not null)
            {
                existingCategoryAttributeEntity.UpdateDetails(name, isVariant);
                return existingCategoryAttributeEntity;
            }
            else
            {
                CategoryAttribute newCategoryAttributeEntity = CategoryAttribute.Create(
                    this.Id,
                    externalId,
                    name,
                    isVariant
                );

                Attributes.Add(newCategoryAttributeEntity);
                return newCategoryAttributeEntity;
            }
        }

        #endregion
    }
}
