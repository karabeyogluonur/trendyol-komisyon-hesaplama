using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class FinancialTransaction : BaseEntity, IEntity
    {
        public int MarketplaceAccountId { get; set; }
        public string MarketplaceTransactionId { get; set; } = string.Empty;
        public FinancialTransactionType TransactionType { get; set; }
        public string? OrderNumber { get; set; }
        public string? OrderItemBarcode { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal CommissionRate { get; set; }
        public string? Description { get; set; }
    }
}
