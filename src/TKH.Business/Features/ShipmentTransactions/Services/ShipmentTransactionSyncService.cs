using Microsoft.Extensions.DependencyInjection;

using TKH.Business.Features.FinancialTransactions.Dtos;
using TKH.Business.Features.FinancialTransactions.Services;
using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Business.Integrations.Marketplaces.Factories;
using TKH.Core.Common.Constants;
using TKH.Core.DataAccess;
using TKH.Entities;
using TKH.Entities.Enums;

namespace TKH.Business.Features.ShipmentTransactions.Services
{
    public class ShipmentTransactionSyncService : IShipmentTransactionSyncService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;

        public ShipmentTransactionSyncService(
            IServiceScopeFactory serviceScopeFactory,
            MarketplaceProviderFactory marketplaceProviderFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _marketplaceProviderFactory = marketplaceProviderFactory;
        }

        public async Task SyncShipmentTransactionsFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto)
        {
            IMarketplaceFinanceProvider marketplaceFinanceProvider = _marketplaceProviderFactory.GetProvider<IMarketplaceFinanceProvider>(marketplaceAccountConnectionDetailsDto.MarketplaceType);

            List<MarketplaceShipmentSyncResultDto> marketplaceShipmentSyncResultDtoBufferList = new List<MarketplaceShipmentSyncResultDto>(ApplicationDefaults.FinanceBatchSize);

            await foreach (MarketplaceShipmentSyncResultDto marketplaceShipmentSyncResultDto in marketplaceFinanceProvider.GetShipmentTransactionsStreamAsync(marketplaceAccountConnectionDetailsDto))
            {
                marketplaceShipmentSyncResultDtoBufferList.Add(marketplaceShipmentSyncResultDto);

                if (marketplaceShipmentSyncResultDtoBufferList.Count >= ApplicationDefaults.FinanceBatchSize)
                {
                    await ProcessShipmentTransactionBatchAsync(marketplaceShipmentSyncResultDtoBufferList, marketplaceAccountConnectionDetailsDto.Id);
                    marketplaceShipmentSyncResultDtoBufferList.Clear();
                }
            }

            if (marketplaceShipmentSyncResultDtoBufferList.Count > 0)
                await ProcessShipmentTransactionBatchAsync(marketplaceShipmentSyncResultDtoBufferList, marketplaceAccountConnectionDetailsDto.Id);
        }

        private async Task ProcessShipmentTransactionBatchAsync(List<MarketplaceShipmentSyncResultDto> marketplaceShipmentSyncResultDtos, int marketplaceAccountId)
        {
            using (IServiceScope serviceScope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork scopedUnitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<ShipmentTransaction> scopedShipmentTransactionRepository = scopedUnitOfWork.GetRepository<ShipmentTransaction>();
                IFinancialTransactionService scopedFinancialTransactionService = serviceScope.ServiceProvider.GetRequiredService<IFinancialTransactionService>();

                List<MarketplaceShipmentTransactionDto> validShipmentTransactionDtos = marketplaceShipmentSyncResultDtos
                        .Where(syncResult => syncResult.ResultStatus == ShipmentTransactionSyncStatus.Synced && syncResult.Shipments.Any())
                        .SelectMany(syncResult => syncResult.Shipments)
                        .ToList();

                if (validShipmentTransactionDtos.Count > 0)
                {
                    List<string> incomingParcelIdList = validShipmentTransactionDtos
                        .Select(dto => dto.ExternalParcelId)
                        .Where(externalParcelId => !string.IsNullOrEmpty(externalParcelId))
                        .ToList();

                    IList<ShipmentTransaction> existingShipmentTransactionList = await scopedShipmentTransactionRepository.GetAllAsync(
                            predicate: shipmentTransaction => shipmentTransaction.MarketplaceAccountId == marketplaceAccountId &&
                                                              incomingParcelIdList.Contains(shipmentTransaction.ExternalParcelId),
                            disableTracking: true,
                            ignoreQueryFilters: true
                    );

                    HashSet<string> existingParcelIdHashSet = existingShipmentTransactionList
                        .Select(shipmentTransaction => shipmentTransaction.ExternalParcelId)
                        .ToHashSet();

                    List<ShipmentTransaction> newShipmentTransactionsToAddList = new List<ShipmentTransaction>();

                    foreach (MarketplaceShipmentTransactionDto marketplaceShipmentTransactionDto in validShipmentTransactionDtos)
                    {
                        if (!existingParcelIdHashSet.Contains(marketplaceShipmentTransactionDto.ExternalParcelId))
                        {
                            ShipmentTransaction newShipmentTransactionEntity = ShipmentTransaction.Create(
                                marketplaceAccountId,
                                marketplaceShipmentTransactionDto.ExternalOrderNumber,
                                marketplaceShipmentTransactionDto.ExternalParcelId,
                                marketplaceShipmentTransactionDto.Amount,
                                marketplaceShipmentTransactionDto.Deci
                            );

                            newShipmentTransactionsToAddList.Add(newShipmentTransactionEntity);
                        }
                    }

                    if (newShipmentTransactionsToAddList.Count > 0)
                        await scopedShipmentTransactionRepository.InsertAsync(newShipmentTransactionsToAddList);
                }

                List<ShipmentSyncStatusUpdateDto> statusUpdateDtos = marketplaceShipmentSyncResultDtos.Select(syncResult => new ShipmentSyncStatusUpdateDto
                {
                    ExternalTransactionId = syncResult.ExternalTransactionId,
                    NewStatus = syncResult.ResultStatus
                }).ToList();

                if (statusUpdateDtos.Count > 0)
                    await scopedFinancialTransactionService.UpdateShipmentSyncStatusesAsync(marketplaceAccountId, statusUpdateDtos);

                await scopedUnitOfWork.SaveChangesAsync();
            }
        }
    }
}
