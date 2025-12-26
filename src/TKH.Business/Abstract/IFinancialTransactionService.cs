using TKH.Business.Dtos.FinancialTransaction;

namespace TKH.Business.Abstract
{
    public interface IFinancialTransactionService
    {
        Task<IList<string>> GetPendingShipmentSyncTransactionIdsAsync(int marketplaceAccountId);
        Task BulkUpdateShipmentSyncStatusAsync(int marketplaceAccountId, List<ShipmentSyncStatusUpdateDto> shipmentSyncStatusUpdateDtos);
    }
}
