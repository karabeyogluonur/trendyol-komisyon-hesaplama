using TKH.Business.Features.FinancialTransactions.Dtos;

namespace TKH.Business.Features.FinancialTransactions.Services
{
    public interface IFinancialTransactionService
    {
        Task<IList<string>> GetPendingShipmentSyncTransactionIdsAsync(int marketplaceAccountId);
        Task UpdateShipmentSyncStatusesAsync(int marketplaceAccountId, List<ShipmentSyncStatusUpdateDto> shipmentSyncStatusUpdateDtos);
    }
}
