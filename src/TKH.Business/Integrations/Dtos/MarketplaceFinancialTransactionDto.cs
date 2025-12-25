using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Dtos
{
    public class MarketplaceFinancialTransactionDto
    {
        public int MarketplaceAccountId { get; set; }
        public string MarketplaceTransactionId { get; set; } = string.Empty;
        public string? OrderNumber { get; set; }
        public FinancialTransactionType TransactionType { get; set; }
        public string MarketplaceTransactionType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
