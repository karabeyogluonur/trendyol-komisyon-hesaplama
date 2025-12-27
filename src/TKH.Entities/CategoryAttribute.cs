using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;

namespace TKH.Entities
{
    public class CategoryAttribute : BaseEntity, IEntity
    {
        public int CategoryId { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsVariant { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<AttributeValue> Values { get; set; } = new List<AttributeValue>();

        public virtual ICollection<ProductAttribute> ProductAttributes { get; set; } = new List<ProductAttribute>();
    }
}
