using AutoMapper;
using System.Runtime.CompilerServices;
using TKH.Business.Integrations.Abstract;
using TKH.Business.Integrations.Dtos;
using TKH.Business.Integrations.Providers.Trendyol;
using TKH.Business.Integrations.Providers.Trendyol.Services;
using Refit;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Business.Integrations.Providers.Trendyol.Enums;
using TKH.Business.Integrations.Providers.Trendyol.Models;

namespace TKH.Business.Integrations.Concrete
{
    public class TrendyolFinanceProvider : IMarketplaceFinanceProvider
    {
        private readonly TrendyolClientFactory _trendyolClientFactory;
        private readonly IMapper _mapper;

        public TrendyolFinanceProvider(
            TrendyolClientFactory trendyolClientFactory,
            IMapper mapper)
        {
            _trendyolClientFactory = trendyolClientFactory;
            _mapper = mapper;
        }

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
                        yield return marketplaceFinancialTransactionDto;
                    }

                    if (apiResponse.Content.Content.Count < TrendyolDefaults.FinancePageSize)
                        hasMoreTransactionsToFetch = false;
                    else
                    {
                        currentPageIndex++;
                        await Task.Delay(TrendyolDefaults.ApiRateLimitDelayMs, cancellationToken);
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
                        yield return marketplaceFinancialTransactionDto;
                    }

                    if (apiResponse.Content.Content.Count < TrendyolDefaults.FinancePageSize)
                        hasMoreTransactionsToFetch = false;
                    else
                    {
                        currentPageIndex++;
                        await Task.Delay(TrendyolDefaults.ApiRateLimitDelayMs, cancellationToken);
                    }
                }
            }
        }
    }
}
