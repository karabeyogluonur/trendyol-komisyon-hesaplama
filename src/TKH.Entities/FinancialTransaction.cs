using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class FinancialTransaction : BaseEntity, IEntity
    {
        public int MarketplaceAccountId { get; set; }
        public string MarketplaceTransactionId { get; set; } = string.Empty;
        public string? OrderNumber { get; set; }
        public FinancialTransactionType TransactionType { get; set; }
        public string MarketplaceTransactionType { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? Title { get; set; }
        public ShipmentTransactionSyncStatus ShipmentTransactionSyncStatus { get; set; } = ShipmentTransactionSyncStatus.Pending;
    }
}
