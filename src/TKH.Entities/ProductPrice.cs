using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class ProductPrice : BaseEntity, IEntity
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public GenerationType GenerationType { get; set; }
        public ProductPriceType Type { get; set; }
        public decimal Amount { get; set; }
        public bool IsVatIncluded { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
    }
}
