using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using TKH.Business.Abstract;
using TKH.Business.Dtos.FinancialTransaction;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Business.Integrations.Abstract;
using TKH.Business.Integrations.Dtos;
using TKH.Business.Integrations.Factories;
using TKH.Core.Common.Constants;
using TKH.Core.DataAccess;
using TKH.Entities;
using TKH.Entities.Enums;

namespace TKH.Business.Concrete
{
    public class FinanceSyncService : IFinanceSyncService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;
        private readonly IMapper _mapper;

        public FinanceSyncService(
            IServiceScopeFactory serviceScopeFactory,
            MarketplaceProviderFactory marketplaceProviderFactory,
            IMapper mapper)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _marketplaceProviderFactory = marketplaceProviderFactory;
            _mapper = mapper;
        }

        public async Task SyncFinancialTransactionsFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto)
        {
            IMarketplaceFinanceProvider marketplaceFinanceProvider = _marketplaceProviderFactory.GetProvider<IMarketplaceFinanceProvider>(marketplaceAccountConnectionDetailsDto.MarketplaceType);

            List<MarketplaceFinancialTransactionDto> marketplaceFinancialTransactionDtoBuffer = new List<MarketplaceFinancialTransactionDto>(ApplicationDefaults.FinanceBatchSize);

            await foreach (MarketplaceFinancialTransactionDto incomingMarketplaceFinancialTransactionDto in marketplaceFinanceProvider.GetFinancialTransactionsStreamAsync(marketplaceAccountConnectionDetailsDto))
            {
                marketplaceFinancialTransactionDtoBuffer.Add(incomingMarketplaceFinancialTransactionDto);

                if (marketplaceFinancialTransactionDtoBuffer.Count >= ApplicationDefaults.FinanceBatchSize)
                {
                    await ProcessFinancialTransactionBatchAsync(marketplaceFinancialTransactionDtoBuffer, marketplaceAccountConnectionDetailsDto.Id);
                    marketplaceFinancialTransactionDtoBuffer.Clear();
                }
            }

            if (marketplaceFinancialTransactionDtoBuffer.Count > 0)
                await ProcessFinancialTransactionBatchAsync(marketplaceFinancialTransactionDtoBuffer, marketplaceAccountConnectionDetailsDto.Id);
        }

        private async Task ProcessFinancialTransactionBatchAsync(List<MarketplaceFinancialTransactionDto> marketplaceFinancialTransactionDtoList, int marketplaceAccountId)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork scopedUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<FinancialTransaction> scopedFinancialTransactionRepository = scopedUnitOfWork.GetRepository<FinancialTransaction>();

                List<string> incomingMarketplaceTransactionIdList = marketplaceFinancialTransactionDtoList
                    .Select(marketplaceFinancialTransactionDto => marketplaceFinancialTransactionDto.MarketplaceTransactionId)
                    .Where(marketplaceTransactionId => !string.IsNullOrEmpty(marketplaceTransactionId))
                    .ToList();

                IList<FinancialTransaction> existingFinancialTransactionList = await scopedFinancialTransactionRepository.GetAllAsync(
                    predicate: financialTransaction => financialTransaction.MarketplaceAccountId == marketplaceAccountId && incomingMarketplaceTransactionIdList.Contains(financialTransaction.MarketplaceTransactionId),
                    disableTracking: false
                );

                Dictionary<string, FinancialTransaction> existingTransactionMap = existingFinancialTransactionList
                    .GroupBy(transaction => transaction.MarketplaceTransactionId)
                    .ToDictionary(group => group.Key, group => group.First());

                List<FinancialTransaction> newFinancialTransactionsToAdd = new List<FinancialTransaction>();

                foreach (MarketplaceFinancialTransactionDto marketplaceFinancialTransactionDto in marketplaceFinancialTransactionDtoList)
                {
                    if (existingTransactionMap.TryGetValue(marketplaceFinancialTransactionDto.MarketplaceTransactionId, out FinancialTransaction? existingFinancialTransaction))
                    {
                        _mapper.Map(marketplaceFinancialTransactionDto, existingFinancialTransaction);
                        existingFinancialTransaction.MarketplaceAccountId = marketplaceAccountId;
                    }
                    else
                    {
                        FinancialTransaction newFinancialTransaction = _mapper.Map<FinancialTransaction>(marketplaceFinancialTransactionDto);
                        newFinancialTransaction.MarketplaceAccountId = marketplaceAccountId;
                        newFinancialTransactionsToAdd.Add(newFinancialTransaction);
                    }
                }

                if (newFinancialTransactionsToAdd.Count > 0)
                    await scopedFinancialTransactionRepository.InsertAsync(newFinancialTransactionsToAdd);

                await scopedUnitOfWork.SaveChangesAsync();
            }
        }

        public async Task SyncShipmentTransactionsFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto)
        {
            IMarketplaceFinanceProvider marketplaceFinanceProvider = _marketplaceProviderFactory.GetProvider<IMarketplaceFinanceProvider>(marketplaceAccountConnectionDetailsDto.MarketplaceType);

            List<MarketplaceShipmentSyncResultDto> marketplaceShipmentSyncResultDtoBuffer = new List<MarketplaceShipmentSyncResultDto>(ApplicationDefaults.FinanceBatchSize);

            await foreach (MarketplaceShipmentSyncResultDto marketplaceShipmentSyncResultDto in marketplaceFinanceProvider.GetShipmentTransactionsStreamAsync(marketplaceAccountConnectionDetailsDto))
            {
                marketplaceShipmentSyncResultDtoBuffer.Add(marketplaceShipmentSyncResultDto);

                if (marketplaceShipmentSyncResultDtoBuffer.Count >= ApplicationDefaults.FinanceBatchSize)
                {
                    await ProcessShipmentTransactionBatchAsync(marketplaceShipmentSyncResultDtoBuffer, marketplaceAccountConnectionDetailsDto.Id);
                    marketplaceShipmentSyncResultDtoBuffer.Clear();
                }
            }

            if (marketplaceShipmentSyncResultDtoBuffer.Count > 0)
                await ProcessShipmentTransactionBatchAsync(marketplaceShipmentSyncResultDtoBuffer, marketplaceAccountConnectionDetailsDto.Id);
        }

        private async Task ProcessShipmentTransactionBatchAsync(List<MarketplaceShipmentSyncResultDto> marketplaceShipmentSyncResultDtoList, int marketplaceAccountId)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork scopedUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<ShipmentTransaction> scopedShipmentTransactionRepository = scopedUnitOfWork.GetRepository<ShipmentTransaction>();
                IFinancialTransactionService scopedFinancialTransactionService = scope.ServiceProvider.GetRequiredService<IFinancialTransactionService>();

                List<MarketplaceShipmentTransactionDto> validShipmentTransactionDtoList = marketplaceShipmentSyncResultDtoList
                    .Where(result => result.ResultStatus == ShipmentTransactionSyncStatus.Synced && result.Shipments.Any())
                    .SelectMany(result => result.Shipments)
                    .ToList();

                if (validShipmentTransactionDtoList.Count > 0)
                {
                    List<string> incomingParcelIdList = validShipmentTransactionDtoList
                        .Select(dto => dto.MarketplaceParcelId)
                        .Where(id => !string.IsNullOrEmpty(id))
                        .ToList();

                    IList<ShipmentTransaction> existingShipmentTransactionList = await scopedShipmentTransactionRepository.GetAllAsync(
                        predicate: shipment => shipment.MarketplaceAccountId == marketplaceAccountId && incomingParcelIdList.Contains(shipment.MarketplaceParcelId),
                        disableTracking: true
                    );

                    HashSet<string> existingParcelIdSet = existingShipmentTransactionList
                        .Select(shipment => shipment.MarketplaceParcelId)
                        .ToHashSet();

                    List<ShipmentTransaction> newShipmentTransactionsToAdd = new List<ShipmentTransaction>();

                    foreach (MarketplaceShipmentTransactionDto marketplaceShipmentTransactionDto in validShipmentTransactionDtoList)
                    {
                        if (!existingParcelIdSet.Contains(marketplaceShipmentTransactionDto.MarketplaceParcelId))
                        {
                            ShipmentTransaction newShipmentTransaction = _mapper.Map<ShipmentTransaction>(marketplaceShipmentTransactionDto);
                            newShipmentTransaction.MarketplaceAccountId = marketplaceAccountId;
                            newShipmentTransactionsToAdd.Add(newShipmentTransaction);
                        }
                    }

                    if (newShipmentTransactionsToAdd.Count > 0)
                        await scopedShipmentTransactionRepository.InsertAsync(newShipmentTransactionsToAdd);
                }

                List<ShipmentSyncStatusUpdateDto> shipmentSyncStatusUpdateDtoList = marketplaceShipmentSyncResultDtoList
                    .Select(result => new ShipmentSyncStatusUpdateDto
                    {
                        MarketplaceTransactionId = result.SourceTransactionId,
                        NewStatus = result.ResultStatus
                    })
                    .ToList();

                if (shipmentSyncStatusUpdateDtoList.Count > 0)
                    await scopedFinancialTransactionService.BulkUpdateShipmentSyncStatusAsync(marketplaceAccountId, shipmentSyncStatusUpdateDtoList);

                await scopedUnitOfWork.SaveChangesAsync();
            }
        }

    }
}
