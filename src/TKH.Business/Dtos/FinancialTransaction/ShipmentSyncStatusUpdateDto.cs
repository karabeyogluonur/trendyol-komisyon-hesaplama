using TKH.Entities.Enums;

namespace TKH.Business.Dtos.FinancialTransaction
{
    public class ShipmentSyncStatusUpdateDto
    {
        public string ExternalTransactionId { get; set; } = string.Empty;
        public ShipmentTransactionSyncStatus NewStatus { get; set; }
    }
}
