using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class FinancialTransaction : BaseEntity, IEntity, IHasMarketplaceAccount
    {
        public int MarketplaceAccountId { get; set; }
        public string ExternalTransactionId { get; set; } = string.Empty;
        public string? ExternalOrderNumber { get; set; }
        public FinancialTransactionType TransactionType { get; set; }
        public string ExternalTransactionType { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? Title { get; set; }
        public decimal? CommissionRate { get; set; }
        public ShipmentTransactionSyncStatus ShipmentTransactionSyncStatus { get; set; } = ShipmentTransactionSyncStatus.Pending;
        public virtual MarketplaceAccount MarketplaceAccount { get; set; }
    }
}
