using Microsoft.EntityFrameworkCore;
using TKH.Business.Abstract;
using TKH.Business.Dtos.FinancialTransaction;
using TKH.Core.DataAccess;
using TKH.Entities;
using TKH.Entities.Enums;

namespace TKH.Business.Concrete
{
    public class FinancialTransactionService : IFinancialTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<FinancialTransaction> _financialTransactionRepository;

        public FinancialTransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _financialTransactionRepository = _unitOfWork.GetRepository<FinancialTransaction>();
        }
        public async Task BulkUpdateShipmentSyncStatusAsync(int marketplaceAccountId, List<ShipmentSyncStatusUpdateDto> shipmentSyncStatusUpdateDtos)
        {
            if (shipmentSyncStatusUpdateDtos is null || !shipmentSyncStatusUpdateDtos.Any()) return;

            var groupedshipmentSyncStatusUpdateDtos = shipmentSyncStatusUpdateDtos.GroupBy(shipmentSyncStatusUpdateDto => shipmentSyncStatusUpdateDto.NewStatus).ToList();

            foreach (var groupedshipmentSyncStatusUpdateDto in groupedshipmentSyncStatusUpdateDtos)
            {
                ShipmentTransactionSyncStatus targetStatus = groupedshipmentSyncStatusUpdateDto.Key;
                List<string> transactionIds = groupedshipmentSyncStatusUpdateDto.Select(groupedshipmentSyncStatusUpdateDto => groupedshipmentSyncStatusUpdateDto.ExternalTransactionId).ToList();

                IQueryable<FinancialTransaction> financialTransactions = _financialTransactionRepository.GetAll(predicate: x => x.MarketplaceAccountId == marketplaceAccountId && transactionIds.Contains(x.ExternalTransactionId),
                ignoreQueryFilters: true
                );

                await financialTransactions.ExecuteUpdateAsync(financialTransaction => financialTransaction.SetProperty(p => p.ShipmentTransactionSyncStatus, targetStatus));
            }
        }

        public async Task<IList<string>> GetPendingShipmentSyncTransactionIdsAsync(int marketplaceAccountId)
        {
            return await _financialTransactionRepository.GetAllAsync(
                predicate: financialTransaction => financialTransaction.MarketplaceAccountId == marketplaceAccountId && financialTransaction.ShipmentTransactionSyncStatus == ShipmentTransactionSyncStatus.Pending,
                selector: financialTransaction => financialTransaction.ExternalTransactionId,
                ignoreQueryFilters: true
            );
        }
    }
}
