using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class Category : BaseEntity, IEntity
    {
        public MarketplaceType MarketplaceType { get; set; }
        public string MarketplaceCategoryId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? ParentMarketplaceCategoryId { get; set; }
        public bool IsLeaf { get; set; }
        public decimal? DefaultCommissionRate { get; set; }
        public ICollection<CategoryAttribute> CategoryAttributes { get; set; } = new List<CategoryAttribute>();
    }
}
