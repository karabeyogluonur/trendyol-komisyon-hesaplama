using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class Category : BaseEntity, IEntity
    {
        public MarketplaceType MarketplaceType { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string? ParentExternalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsLeaf { get; set; }
        public decimal? DefaultCommissionRate { get; set; }
        public virtual ICollection<CategoryAttribute> Attributes { get; set; } = new List<CategoryAttribute>();

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
