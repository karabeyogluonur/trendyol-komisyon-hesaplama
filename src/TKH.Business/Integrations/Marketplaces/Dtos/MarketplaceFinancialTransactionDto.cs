using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Marketplaces.Dtos
{
    public class MarketplaceFinancialTransactionDto
    {
        public int MarketplaceAccountId { get; set; }
        public string ExternalTransactionId { get; set; } = string.Empty;
        public string? ExternalOrderNumber { get; set; }
        public FinancialTransactionType TransactionType { get; set; }
        public ShipmentTransactionSyncStatus ShipmentTransactionSyncStatus { get; set; }
        public string ExternalTransactionType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
