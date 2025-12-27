using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;

namespace TKH.Entities
{
    public class AttributeValue : BaseEntity, IEntity
    {
        public int CategoryAttributeId { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public virtual CategoryAttribute Attribute { get; set; }
        public virtual ICollection<ProductAttribute> ProductAttributes { get; set; } = new List<ProductAttribute>();
    }
}
