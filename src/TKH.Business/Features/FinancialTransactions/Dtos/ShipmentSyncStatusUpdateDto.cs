using TKH.Entities.Enums;

namespace TKH.Business.Features.FinancialTransactions.Dtos
{
    public class ShipmentSyncStatusUpdateDto
    {
        public string ExternalTransactionId { get; set; } = string.Empty;
        public ShipmentTransactionSyncStatus NewStatus { get; set; }
    }
}
