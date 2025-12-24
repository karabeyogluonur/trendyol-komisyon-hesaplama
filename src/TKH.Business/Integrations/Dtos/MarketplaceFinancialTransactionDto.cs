using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Dtos
{
    public class MarketplaceFinancialTransactionDto
    {
        public string MarketplaceTransactionId { get; set; } = string.Empty;
        public int MarketplaceAccountId { get; set; }
        public string? OrderNumber { get; set; }
        public string? OrderItemBarcode { get; set; }
        public FinancialTransactionType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal CommissionRate { get; set; }
        public string? Description { get; set; }
    }
}
