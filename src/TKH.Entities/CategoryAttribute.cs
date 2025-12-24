using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;

namespace TKH.Entities
{
    public class CategoryAttribute : BaseEntity, IEntity
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string MarketplaceAttributeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsVarianter { get; set; }
        public ICollection<AttributeValue> AttributeValues { get; set; } = new List<AttributeValue>();
    }
}
