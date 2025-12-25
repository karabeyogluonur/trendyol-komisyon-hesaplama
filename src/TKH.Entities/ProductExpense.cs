using TKH.Entities.Enums;
using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;

namespace TKH.Entities
{
    public class ProductExpense : BaseEntity, IEntity
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public ProductExpenseType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal VatRate { get; set; }
        public bool IsVatIncluded { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
    }
}
