using AutoMapper;
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
        private readonly IMapper _mapper;

        public ShipmentTransactionSyncService(
            IServiceScopeFactory serviceScopeFactory,
            MarketplaceProviderFactory marketplaceProviderFactory,
            IMapper mapper)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _marketplaceProviderFactory = marketplaceProviderFactory;
            _mapper = mapper;
        }

        public async Task SyncShipmentTransactionsFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto)
        {
            IMarketplaceFinanceProvider marketplaceFinanceProvider = _marketplaceProviderFactory.GetProvider<IMarketplaceFinanceProvider>(marketplaceAccountConnectionDetailsDto.MarketplaceType);

            List<MarketplaceShipmentSyncResultDto> buffer =new(ApplicationDefaults.FinanceBatchSize);

            await foreach (MarketplaceShipmentSyncResultDto dto in marketplaceFinanceProvider.GetShipmentTransactionsStreamAsync(marketplaceAccountConnectionDetailsDto))
            {
                buffer.Add(dto);

                if (buffer.Count >= ApplicationDefaults.FinanceBatchSize)
                {
                    await ProcessShipmentTransactionBatchAsync(buffer, marketplaceAccountConnectionDetailsDto.Id);
                    buffer.Clear();
                }
            }

            if (buffer.Count > 0)
                await ProcessShipmentTransactionBatchAsync(buffer, marketplaceAccountConnectionDetailsDto.Id);
        }

        private async Task ProcessShipmentTransactionBatchAsync(
            List<MarketplaceShipmentSyncResultDto> dtoList,
            int marketplaceAccountId)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<ShipmentTransaction> repository =
                    uow.GetRepository<ShipmentTransaction>();
                IFinancialTransactionService financialTransactionService =
                    scope.ServiceProvider.GetRequiredService<IFinancialTransactionService>();

                List<MarketplaceShipmentTransactionDto> validShipments =
                    dtoList
                        .Where(x => x.ResultStatus == ShipmentTransactionSyncStatus.Synced && x.Shipments.Any())
                        .SelectMany(x => x.Shipments)
                        .ToList();

                if (validShipments.Count > 0)
                {
                    List<string> incomingParcelIds =
                        validShipments
                            .Select(x => x.ExternalParcelId)
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToList();

                    IList<ShipmentTransaction> existingList =
                        await repository.GetAllAsync(
                            predicate: x =>
                                x.MarketplaceAccountId == marketplaceAccountId &&
                                incomingParcelIds.Contains(x.ExternalParcelId),
                            disableTracking: true,
                            ignoreQueryFilters: true);

                    HashSet<string> existingSet =
                        existingList.Select(x => x.ExternalParcelId).ToHashSet();

                    List<ShipmentTransaction> newItems = new();

                    foreach (MarketplaceShipmentTransactionDto dto in validShipments)
                    {
                        if (!existingSet.Contains(dto.ExternalParcelId))
                        {
                            ShipmentTransaction entity = _mapper.Map<ShipmentTransaction>(dto);
                            entity.MarketplaceAccountId = marketplaceAccountId;
                            newItems.Add(entity);
                        }
                    }

                    if (newItems.Count > 0)
                        await repository.InsertAsync(newItems);
                }

                List<ShipmentSyncStatusUpdateDto> statusUpdates =
                    dtoList
                        .Select(x => new ShipmentSyncStatusUpdateDto
                        {
                            ExternalTransactionId = x.ExternalTransactionId,
                            NewStatus = x.ResultStatus
                        })
                        .ToList();

                if (statusUpdates.Count > 0)
                    await financialTransactionService
                        .BulkUpdateShipmentSyncStatusAsync(marketplaceAccountId, statusUpdates);

                await uow.SaveChangesAsync();
            }
        }
    }
}
