using AutoMapper;
using TKH.Business.Abstract;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Business.Integrations.Abstract;
using TKH.Business.Integrations.Dtos;
using TKH.Business.Integrations.Factories;
using TKH.Core.Common.Constants;
using TKH.Core.DataAccess;
using TKH.Entities;

namespace TKH.Business.Concrete
{
    public class FinanceSyncService : IFinanceSyncService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<FinancialTransaction> _financialTransactionRepository;
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;
        private readonly IMapper _mapper;

        public FinanceSyncService(
            IUnitOfWork unitOfWork,
            MarketplaceProviderFactory marketplaceProviderFactory,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _financialTransactionRepository = _unitOfWork.GetRepository<FinancialTransaction>();
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
            List<string> incomingMarketplaceTransactionIdList = marketplaceFinancialTransactionDtoList
                .Select(marketplaceFinancialTransactionDto => marketplaceFinancialTransactionDto.MarketplaceTransactionId)
                .Where(marketplaceTransactionId => !string.IsNullOrEmpty(marketplaceTransactionId))
                .ToList();

            List<FinancialTransaction> existingFinancialTransactionList = await _financialTransactionRepository.GetAllAsync(
                financialTransaction => financialTransaction.MarketplaceAccountId == marketplaceAccountId && incomingMarketplaceTransactionIdList.Contains(financialTransaction.MarketplaceTransactionId)
            );

            List<FinancialTransaction> financialTransactionsToProcessList = new List<FinancialTransaction>();

            foreach (MarketplaceFinancialTransactionDto marketplaceFinancialTransactionDto in marketplaceFinancialTransactionDtoList)
            {
                FinancialTransaction existingFinancialTransaction = existingFinancialTransactionList.FirstOrDefault(financialTransaction => financialTransaction.MarketplaceTransactionId == marketplaceFinancialTransactionDto.MarketplaceTransactionId);

                if (existingFinancialTransaction != null)
                {
                    _mapper.Map(marketplaceFinancialTransactionDto, existingFinancialTransaction);
                    financialTransactionsToProcessList.Add(existingFinancialTransaction);
                }
                else
                {
                    FinancialTransaction newFinancialTransaction = _mapper.Map<FinancialTransaction>(marketplaceFinancialTransactionDto);
                    newFinancialTransaction.MarketplaceAccountId = marketplaceAccountId;

                    financialTransactionsToProcessList.Add(newFinancialTransaction);
                }
            }

            _financialTransactionRepository.AddOrUpdate(financialTransactionsToProcessList);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
