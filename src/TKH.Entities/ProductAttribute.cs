using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;

namespace TKH.Entities
{
    public class ProductAttribute : BaseEntity, IEntity
    {
        public int ProductId { get; set; }
        public int CategoryAttributeId { get; set; }
        public int? AttributeValueId { get; set; }
        public string? CustomValue { get; set; }
        public virtual Product Product { get; set; }
        public virtual CategoryAttribute Attribute { get; set; }
        public virtual AttributeValue? Value { get; set; }
    }
}
