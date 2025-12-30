using AutoMapper;
using System.Runtime.CompilerServices;
using Refit;
using TKH.Business.Integrations.Providers.Trendyol;
using TKH.Business.Integrations.Providers.Trendyol.Models;
using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Integrations.Trendyol.Infrastructure;
using TKH.Integrations.Trendyol.HttpClients;
using TKH.Entities.Enums;
using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Dtos;

namespace TKH.Integrations.Trendyol.Providers
{
    public class TrendyolClaimProvider : IMarketplaceClaimProvider
    {
        private readonly TrendyolClientFactory _trendyolClientFactory;
        private readonly IMapper _mapper;

        public TrendyolClaimProvider(
            TrendyolClientFactory trendyolClientFactory,
            IMapper mapper)
        {
            _trendyolClientFactory = trendyolClientFactory;
            _mapper = mapper;
        }

        public MarketplaceType MarketplaceType => MarketplaceType.Trendyol;

        public async IAsyncEnumerable<MarketplaceClaimDto> GetClaimsStreamAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {

            if (!long.TryParse(marketplaceAccountConnectionDetailsDto.MerchantId, out long sellerIdentifier))
                yield break;

            ITrendyolClaimService trendyolClaimApi = _trendyolClientFactory.CreateClient<ITrendyolClaimService>(
                    sellerIdentifier,
                    marketplaceAccountConnectionDetailsDto.ApiKey,
                    marketplaceAccountConnectionDetailsDto.ApiSecretKey);

            DateTimeOffset searchEndDate = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(TrendyolDefaults.TimeZoneOffsetHours));
            DateTimeOffset searchStartDate = searchEndDate.AddMonths(TrendyolDefaults.ClaimSyncLookbackMonths);
            DateTimeOffset currentWindowStartDate = searchStartDate;

            while (currentWindowStartDate < searchEndDate && !cancellationToken.IsCancellationRequested)
            {
                DateTimeOffset currentWindowEndDate = currentWindowStartDate.AddDays(TrendyolDefaults.ClaimSyncDateWindowDays);

                if (currentWindowEndDate > searchEndDate)
                    currentWindowEndDate = searchEndDate;

                long startDateTimestamp = currentWindowStartDate.ToUnixTimeMilliseconds();
                long endDateTimestamp = currentWindowEndDate.ToUnixTimeMilliseconds();

                int currentPageIndex = 0;
                bool hasMoreClaimsToFetch = true;

                while (hasMoreClaimsToFetch && !cancellationToken.IsCancellationRequested)
                {
                    TrendyolClaimSearchRequest request = new TrendyolClaimSearchRequest
                    {
                        Page = currentPageIndex,
                        Size = TrendyolDefaults.OrderPageSize,
                        StartDate = startDateTimestamp,
                        EndDate = endDateTimestamp,
                    };

                    IApiResponse<TrendyolClaimResponse> response = await trendyolClaimApi.GetClaimsAsync(sellerIdentifier, request);

                    if (!response.IsSuccessStatusCode || response.Content?.Content == null || response.Content.Content.Count == 0)
                    {
                        hasMoreClaimsToFetch = false;
                        continue;
                    }

                    foreach (TrendyolClaimContent claimContent in response.Content.Content)
                    {
                        MarketplaceClaimDto marketplaceClaimDto = _mapper.Map<MarketplaceClaimDto>(claimContent);
                        marketplaceClaimDto.MarketplaceAccountId = marketplaceAccountConnectionDetailsDto.Id;

                        yield return marketplaceClaimDto;
                    }

                    if (response.Content.Content.Count < TrendyolDefaults.ClaimPageSize)
                        hasMoreClaimsToFetch = false;
                    else
                    {
                        currentPageIndex++;
                    }
                }
                currentWindowStartDate = currentWindowEndDate;
            }
        }
    }
}
