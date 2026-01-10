using AutoMapper;

using Microsoft.Extensions.DependencyInjection;

using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Business.Integrations.Marketplaces.Factories;
using TKH.Core.Common.Constants;
using TKH.Core.Common.Extensions;
using TKH.Core.DataAccess;
using TKH.Entities;

namespace TKH.Business.Features.FinancialTransactions.Services
{
    public class FinancialTransactionSyncService : IFinancialTransactionSyncService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;
        private readonly IMapper _mapper;

        public FinancialTransactionSyncService(
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
            if (marketplaceFinancialTransactionDtoList is null || !marketplaceFinancialTransactionDtoList.Any())
                return;

            using (IServiceScope serviceScope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork scopedUnitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<FinancialTransaction> scopedFinancialTransactionRepository = scopedUnitOfWork.GetRepository<FinancialTransaction>();

                List<string> incomingMarketplaceTransactionIdList = marketplaceFinancialTransactionDtoList
                    .Select(marketplaceFinancialTransactionDto => marketplaceFinancialTransactionDto.ExternalTransactionId)
                    .Where(marketplaceTransactionId => !string.IsNullOrEmpty(marketplaceTransactionId))
                    .ToList();

                IList<FinancialTransaction> existingFinancialTransactionList = await scopedFinancialTransactionRepository.GetAllAsync(
                    predicate: financialTransaction => financialTransaction.MarketplaceAccountId == marketplaceAccountId &&
                                                       incomingMarketplaceTransactionIdList.Contains(financialTransaction.ExternalTransactionId),
                    disableTracking: false,
                    ignoreQueryFilters: true
                );

                Dictionary<string, FinancialTransaction> existingTransactionMapDictionary = existingFinancialTransactionList
                    .GroupBy(transaction => transaction.ExternalTransactionId)
                    .ToDictionary(group => group.Key, group => group.First());

                List<FinancialTransaction> newFinancialTransactionsToAddList = new List<FinancialTransaction>();

                foreach (MarketplaceFinancialTransactionDto marketplaceFinancialTransactionDto in marketplaceFinancialTransactionDtoList)
                {
                    if (existingTransactionMapDictionary.TryGetValue(marketplaceFinancialTransactionDto.ExternalTransactionId, out FinancialTransaction? existingFinancialTransaction))
                    {
                        existingFinancialTransaction.UpdateDetails(
                            marketplaceFinancialTransactionDto.ExternalOrderNumber,
                            marketplaceFinancialTransactionDto.TransactionType,
                            marketplaceFinancialTransactionDto.ExternalTransactionType,
                            marketplaceFinancialTransactionDto.TransactionDate.EnsureUtc(),
                            marketplaceFinancialTransactionDto.Amount,
                            marketplaceFinancialTransactionDto.Description,
                            marketplaceFinancialTransactionDto.Title,
                            marketplaceFinancialTransactionDto.CommissionRate
                        );
                    }
                    else
                    {
                        FinancialTransaction newFinancialTransaction = FinancialTransaction.Create(
                            marketplaceAccountId,
                            marketplaceFinancialTransactionDto.ExternalTransactionId,
                            marketplaceFinancialTransactionDto.ExternalOrderNumber,
                            marketplaceFinancialTransactionDto.TransactionType,
                            marketplaceFinancialTransactionDto.ExternalTransactionType,
                            marketplaceFinancialTransactionDto.TransactionDate.EnsureUtc(),
                            marketplaceFinancialTransactionDto.Amount,
                            marketplaceFinancialTransactionDto.Description,
                            marketplaceFinancialTransactionDto.Title,
                            marketplaceFinancialTransactionDto.CommissionRate
                        );

                        newFinancialTransactionsToAddList.Add(newFinancialTransaction);
                    }
                }

                if (newFinancialTransactionsToAddList.Count > 0)
                    await scopedFinancialTransactionRepository.InsertAsync(newFinancialTransactionsToAddList);

                await scopedUnitOfWork.SaveChangesAsync();
            }
        }


    }
}
