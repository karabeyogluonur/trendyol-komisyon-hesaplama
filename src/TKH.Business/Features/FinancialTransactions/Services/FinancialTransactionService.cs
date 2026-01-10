using Microsoft.Extensions.Logging;

using TKH.Business.Features.FinancialTransactions.Dtos;
using TKH.Core.DataAccess;
using TKH.Entities;
using TKH.Entities.Enums;

namespace TKH.Business.Features.FinancialTransactions.Services
{
    public class FinancialTransactionService : IFinancialTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<FinancialTransaction> _financialTransactionRepository;
        private readonly ILogger<FinancialTransactionService> _logger;

        public FinancialTransactionService(
            IUnitOfWork unitOfWork,
            ILogger<FinancialTransactionService> logger)
        {
            _unitOfWork = unitOfWork;
            _financialTransactionRepository = _unitOfWork.GetRepository<FinancialTransaction>();
            _logger = logger;
        }

        public async Task UpdateShipmentSyncStatusesAsync(int marketplaceAccountId, List<ShipmentSyncStatusUpdateDto> shipmentSyncStatusUpdateDtos)
        {
            if (shipmentSyncStatusUpdateDtos is null || !shipmentSyncStatusUpdateDtos.Any())
            {
                _logger.LogWarning("UpdateShipmentSyncStatusesRangeAsync called with empty list for AccountId: {AccountId}", marketplaceAccountId);
                return;
            }

            _logger.LogInformation("Bulk updating shipment sync statuses. Count: {Count}, AccountId: {AccountId}", shipmentSyncStatusUpdateDtos.Count, marketplaceAccountId);

            var groupedShipmentSyncStatusUpdateDtos = shipmentSyncStatusUpdateDtos
                .GroupBy(shipmentSyncStatusUpdateDto => shipmentSyncStatusUpdateDto.NewStatus)
                .ToList();

            int totalUpdatedCount = 0;

            foreach (var groupedDto in groupedShipmentSyncStatusUpdateDtos)
            {
                ShipmentTransactionSyncStatus targetShipmentTransactionSyncStatus = groupedDto.Key;
                List<string> externalTransactionIds = groupedDto.Select(shipmentSyncStatusUpdateDto => shipmentSyncStatusUpdateDto.ExternalTransactionId).ToList();

                IList<FinancialTransaction> financialTransactions = await _financialTransactionRepository.GetAllAsync(
                    predicate: financialTransaction => financialTransaction.MarketplaceAccountId == marketplaceAccountId &&
                                                       externalTransactionIds.Contains(financialTransaction.ExternalTransactionId),
                    disableTracking: false,
                    ignoreQueryFilters: true
                );

                if (!financialTransactions.Any())
                    continue;

                foreach (FinancialTransaction financialTransaction in financialTransactions)
                {
                    financialTransaction.UpdateShipmentSyncStatus(targetShipmentTransactionSyncStatus);
                    totalUpdatedCount++;
                }
            }

            if (totalUpdatedCount > 0)
            {
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Successfully updated sync status for {Count} transactions.", totalUpdatedCount);
            }
            else
                _logger.LogInformation("No transactions were updated.");
        }

        public async Task<IList<string>> GetPendingShipmentSyncTransactionIdsAsync(int marketplaceAccountId)
        {
            return await _financialTransactionRepository.GetAllAsync(
                predicate: financialTransaction => financialTransaction.MarketplaceAccountId == marketplaceAccountId &&
                                                   financialTransaction.ShipmentTransactionSyncStatus == ShipmentTransactionSyncStatus.Pending,
                selector: financialTransaction => financialTransaction.ExternalTransactionId,
                ignoreQueryFilters: true
            );
        }
    }
}
