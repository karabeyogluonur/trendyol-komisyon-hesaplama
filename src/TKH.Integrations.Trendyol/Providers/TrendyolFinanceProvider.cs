using AutoMapper;
using System.Runtime.CompilerServices;
using Refit;
using TKH.Entities.Enums;
using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Business.Features.FinancialTransactions.Services;
using TKH.Integrations.Trendyol.Infrastructure;
using TKH.Integrations.Trendyol.HttpClients;
using TKH.Business.Integrations.Providers.Trendyol;
using TKH.Integrations.Trendyol.Enums;
using TKH.Business.Integrations.Providers.Trendyol.Models;
using TKH.Integrations.Trendyol.Extensions;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Business.Integrations.Marketplaces.Abstract;

namespace TKH.Integrations.Trendyol.Providers
{
    public class TrendyolFinanceProvider : IMarketplaceFinanceProvider
    {
        private readonly TrendyolClientFactory _trendyolClientFactory;
        private readonly IFinancialTransactionService _financialTransactionService;
        private readonly IMapper _mapper;

        public TrendyolFinanceProvider(
            TrendyolClientFactory trendyolClientFactory,
            IFinancialTransactionService financialTransactionService,
            IMapper mapper)
        {
            _financialTransactionService = financialTransactionService;
            _trendyolClientFactory = trendyolClientFactory;
            _mapper = mapper;
        }

        public MarketplaceType MarketplaceType => MarketplaceType.Trendyol;

        public async IAsyncEnumerable<MarketplaceFinancialTransactionDto> GetFinancialTransactionsStreamAsync(
            MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (!long.TryParse(marketplaceAccountConnectionDetailsDto.MerchantId, out long sellerIdentifier))
                yield break;

            ITrendyolFinanceService trendyolFinanceApi = _trendyolClientFactory.CreateClient<ITrendyolFinanceService>(
                    sellerIdentifier,
                    marketplaceAccountConnectionDetailsDto.ApiKey,
                    marketplaceAccountConnectionDetailsDto.ApiSecretKey);

            DateTimeOffset searchEndDate = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(TrendyolDefaults.TimeZoneOffsetHours));
            DateTimeOffset searchStartDate = searchEndDate.AddMonths(TrendyolDefaults.FinanceSyncLookbackMonths);

            DateTimeOffset currentWindowStartDate = searchStartDate;

            while (currentWindowStartDate < searchEndDate && !cancellationToken.IsCancellationRequested)
            {
                DateTimeOffset currentWindowEndDate = currentWindowStartDate.AddDays(TrendyolDefaults.FinanceSyncDateWindowDays);

                if (currentWindowEndDate > searchEndDate)
                    currentWindowEndDate = searchEndDate;

                long startDateTimestamp = currentWindowStartDate.ToUnixTimeMilliseconds();
                long endDateTimestamp = currentWindowEndDate.ToUnixTimeMilliseconds();

                await foreach (MarketplaceFinancialTransactionDto marketplaceFinancialTransactionDto in GetSettlementTransactionsStreamAsync(trendyolFinanceApi, sellerIdentifier, marketplaceAccountConnectionDetailsDto.Id, startDateTimestamp, endDateTimestamp, cancellationToken))
                {
                    yield return marketplaceFinancialTransactionDto;
                }
                await foreach (MarketplaceFinancialTransactionDto marketplaceFinancialTransactionDto in GetOtherFinancialTransactionsStreamAsync(trendyolFinanceApi, sellerIdentifier, marketplaceAccountConnectionDetailsDto.Id, startDateTimestamp, endDateTimestamp, cancellationToken))
                {
                    yield return marketplaceFinancialTransactionDto;
                }

                currentWindowStartDate = currentWindowEndDate;
            }
        }

        private async IAsyncEnumerable<MarketplaceFinancialTransactionDto> GetSettlementTransactionsStreamAsync(
            ITrendyolFinanceService trendyolFinanceApi,
            long sellerIdentifier,
            int marketplaceAccountId,
            long startDateTimestamp,
            long endDateTimestamp,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (TrendyolSettlementTransactionType settlementTransactionType in Enum.GetValues(typeof(TrendyolSettlementTransactionType)))
            {
                int currentPageIndex = 0;
                bool hasMoreTransactionsToFetch = true;

                while (hasMoreTransactionsToFetch && !cancellationToken.IsCancellationRequested)
                {
                    TrendyolSettlementSearchRequest trendyolSettlementSearchRequest = new TrendyolSettlementSearchRequest
                    {
                        Page = currentPageIndex,
                        Size = TrendyolDefaults.FinancePageSize,
                        StartDate = startDateTimestamp,
                        EndDate = endDateTimestamp,
                        SupplierId = sellerIdentifier,
                        TransactionType = settlementTransactionType
                    };

                    IApiResponse<TrendyolFinancialResponse> apiResponse = await trendyolFinanceApi.GetSettlementsAsync(sellerIdentifier, trendyolSettlementSearchRequest);

                    if (!apiResponse.IsSuccessStatusCode ||
                        apiResponse.Content?.Content == null ||
                        apiResponse.Content.Content.Count == 0)
                    {
                        hasMoreTransactionsToFetch = false;
                        continue;
                    }

                    foreach (TrendyolFinancialContent trendyolFinancialContent in apiResponse.Content.Content)
                    {
                        MarketplaceFinancialTransactionDto marketplaceFinancialTransactionDto = _mapper.Map<MarketplaceFinancialTransactionDto>(trendyolFinancialContent);
                        marketplaceFinancialTransactionDto.MarketplaceAccountId = marketplaceAccountId;
                        marketplaceFinancialTransactionDto.ExternalTransactionType = settlementTransactionType.ToString();
                        marketplaceFinancialTransactionDto.TransactionType = settlementTransactionType.ToFinancialTransactionType();
                        marketplaceFinancialTransactionDto.ShipmentTransactionSyncStatus = ShipmentTransactionSyncStatus.NotRequired;
                        yield return marketplaceFinancialTransactionDto;
                    }

                    if (apiResponse.Content.Content.Count < TrendyolDefaults.FinancePageSize)
                        hasMoreTransactionsToFetch = false;
                    else
                    {
                        currentPageIndex++;
                    }
                }
            }
        }

        private async IAsyncEnumerable<MarketplaceFinancialTransactionDto> GetOtherFinancialTransactionsStreamAsync(
            ITrendyolFinanceService trendyolFinanceApi,
            long sellerIdentifier,
            int marketplaceAccountId,
            long startDateTimestamp,
            long endDateTimestamp,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (TrendyolOtherFinancialTransactionType otherFinancialTransactionType in Enum.GetValues(typeof(TrendyolOtherFinancialTransactionType)))
            {
                int currentPageIndex = 0;
                bool hasMoreTransactionsToFetch = true;

                while (hasMoreTransactionsToFetch && !cancellationToken.IsCancellationRequested)
                {
                    TrendyolOtherFinancialSearchRequest trendyolOtherFinancialSearchRequest = new TrendyolOtherFinancialSearchRequest
                    {
                        Page = currentPageIndex,
                        Size = TrendyolDefaults.FinancePageSize,
                        StartDate = startDateTimestamp,
                        EndDate = endDateTimestamp,
                        SupplierId = sellerIdentifier,
                        TransactionType = otherFinancialTransactionType
                    };

                    IApiResponse<TrendyolFinancialResponse> apiResponse = await trendyolFinanceApi.GetOtherFinancialsAsync(sellerIdentifier, trendyolOtherFinancialSearchRequest);

                    if (!apiResponse.IsSuccessStatusCode ||
                        apiResponse.Content?.Content == null ||
                        apiResponse.Content.Content.Count == 0)
                    {
                        hasMoreTransactionsToFetch = false;
                        continue;
                    }

                    foreach (TrendyolFinancialContent trendyolFinancialContent in apiResponse.Content.Content)
                    {
                        MarketplaceFinancialTransactionDto marketplaceFinancialTransactionDto = _mapper.Map<MarketplaceFinancialTransactionDto>(trendyolFinancialContent);
                        marketplaceFinancialTransactionDto.MarketplaceAccountId = marketplaceAccountId;
                        marketplaceFinancialTransactionDto.ExternalTransactionType = otherFinancialTransactionType.ToString();
                        marketplaceFinancialTransactionDto.TransactionType = otherFinancialTransactionType.ToFinancialTransactionType();

                        bool isFinancialCargoTransaction = otherFinancialTransactionType == TrendyolOtherFinancialTransactionType.DeductionInvoices && !string.IsNullOrEmpty(trendyolFinancialContent.TransactionType) && trendyolFinancialContent.TransactionType.Contains("Kargo", StringComparison.OrdinalIgnoreCase);

                        marketplaceFinancialTransactionDto.ShipmentTransactionSyncStatus = isFinancialCargoTransaction ? ShipmentTransactionSyncStatus.Pending : ShipmentTransactionSyncStatus.NotRequired;
                        yield return marketplaceFinancialTransactionDto;
                    }

                    if (apiResponse.Content.Content.Count < TrendyolDefaults.FinancePageSize)
                        hasMoreTransactionsToFetch = false;
                    else
                    {
                        currentPageIndex++;
                    }
                }
            }
        }

        public async IAsyncEnumerable<MarketplaceShipmentSyncResultDto> GetShipmentTransactionsStreamAsync(
            MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (!long.TryParse(marketplaceAccountConnectionDetailsDto.MerchantId, out long sellerIdentifier))
                yield break;

            IList<string> pendingTransactionIdList = await _financialTransactionService.GetPendingShipmentSyncTransactionIdsAsync(marketplaceAccountConnectionDetailsDto.Id);

            if (!pendingTransactionIdList.Any())
                yield break;

            ITrendyolFinanceService trendyolFinanceService = _trendyolClientFactory.CreateClient<ITrendyolFinanceService>(
                    sellerIdentifier,
                    marketplaceAccountConnectionDetailsDto.ApiKey,
                    marketplaceAccountConnectionDetailsDto.ApiSecretKey);

            foreach (string pendingTransactionId in pendingTransactionIdList)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                MarketplaceShipmentSyncResultDto marketplaceShipmentSyncResultDto = new MarketplaceShipmentSyncResultDto
                {
                    ExternalTransactionId = pendingTransactionId,
                    ResultStatus = ShipmentTransactionSyncStatus.Failed
                };

                IApiResponse<TrendyolCargoInvoiceResponse> trendyolCargoInvoiceResponse = await trendyolFinanceService.GetCargoInvoiceAsync(sellerIdentifier, pendingTransactionId);

                if (trendyolCargoInvoiceResponse.IsSuccessStatusCode)
                {
                    marketplaceShipmentSyncResultDto.ResultStatus = ShipmentTransactionSyncStatus.Synced;

                    if (trendyolCargoInvoiceResponse.Content?.Content != null && trendyolCargoInvoiceResponse.Content.Content.Count > 0)
                        marketplaceShipmentSyncResultDto.Shipments = _mapper.Map<List<MarketplaceShipmentTransactionDto>>(trendyolCargoInvoiceResponse.Content.Content);
                }

                yield return marketplaceShipmentSyncResultDto;
            }
        }
    }
}
