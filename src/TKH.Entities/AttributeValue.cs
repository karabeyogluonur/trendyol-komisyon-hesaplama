using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;

namespace TKH.Entities
{
    public class AttributeValue : BaseEntity, IEntity
    {
        public int CategoryAttributeId { get; set; }
        public virtual CategoryAttribute CategoryAttribute { get; set; }
        public string MarketplaceValueId { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
