using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;

namespace TKH.Entities
{
    public class ProductAttribute : BaseEntity, IEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string MarketplaceAttributeId { get; set; } = string.Empty;
        public string AttributeName { get; set; } = string.Empty;
        public string? MarketplaceAttributeValueId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
