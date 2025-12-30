using AutoMapper;
using System.Runtime.CompilerServices;
using TKH.Business.Integrations.Providers.Trendyol;
using Refit;
using TKH.Business.Integrations.Providers.Trendyol.Models;
using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Integrations.Trendyol.Infrastructure;
using TKH.Integrations.Trendyol.HttpClients;
using TKH.Integrations.Trendyol.Enums;
using TKH.Entities.Enums;
using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Dtos;

namespace TKH.Integrations.Trendyol.Providers
{
    public class TrendyolOrderProvider : IMarketplaceOrderProvider
    {
        private readonly TrendyolClientFactory _trendyolClientFactory;
        private readonly IMapper _mapper;

        public TrendyolOrderProvider(
            TrendyolClientFactory trendyolClientFactory,
            IMapper mapper)
        {
            _trendyolClientFactory = trendyolClientFactory;
            _mapper = mapper;
        }

        public MarketplaceType MarketplaceType => MarketplaceType.Trendyol;

        public async IAsyncEnumerable<MarketplaceOrderDto> GetOrdersStreamAsync(
            MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (!long.TryParse(marketplaceAccountConnectionDetailsDto.MerchantId, out long sellerIdentifier))
                yield break;

            ITrendyolOrderService trendyolOrderApi =
                _trendyolClientFactory.CreateClient<ITrendyolOrderService>(
                    sellerIdentifier,
                    marketplaceAccountConnectionDetailsDto.ApiKey,
                    marketplaceAccountConnectionDetailsDto.ApiSecretKey);


            DateTimeOffset searchEndDate = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(TrendyolDefaults.TimeZoneOffsetHours));
            DateTimeOffset searchStartDate = searchEndDate.AddMonths(TrendyolDefaults.OrderSyncLookbackMonths);
            DateTimeOffset currentWindowStartDate = searchStartDate;

            while (currentWindowStartDate < searchEndDate && !cancellationToken.IsCancellationRequested)
            {
                DateTimeOffset currentWindowEndDate = currentWindowStartDate.AddDays(TrendyolDefaults.OrderSyncDateWindowDays);

                if (currentWindowEndDate > searchEndDate)
                    currentWindowEndDate = searchEndDate;

                long startDateTimestamp = currentWindowStartDate.ToUnixTimeMilliseconds();
                long endDateTimestamp = currentWindowEndDate.ToUnixTimeMilliseconds();

                int currentPageIndex = 0;
                bool hasMoreOrdersToFetch = true;

                while (hasMoreOrdersToFetch && !cancellationToken.IsCancellationRequested)
                {
                    TrendyolOrderSearchRequest request = new TrendyolOrderSearchRequest
                    {
                        Page = currentPageIndex,
                        Size = TrendyolDefaults.OrderPageSize,
                        StartDate = startDateTimestamp,
                        EndDate = endDateTimestamp,
                        OrderByField = TrendyolOrderByField.CreatedDate,
                        OrderByDirection = TrendyolOrderByDirection.DESC
                    };

                    IApiResponse<TrendyolOrderResponse> response =
                        await trendyolOrderApi.GetOrdersAsync(sellerIdentifier, request);

                    if (!response.IsSuccessStatusCode ||
                        response.Content?.Content == null ||
                        response.Content.Content.Count == 0)
                    {
                        hasMoreOrdersToFetch = false;
                        continue;
                    }

                    foreach (TrendyolOrderContent order in response.Content.Content)
                    {
                        MarketplaceOrderDto dto = _mapper.Map<MarketplaceOrderDto>(order);
                        dto.MarketplaceAccountId = marketplaceAccountConnectionDetailsDto.Id;

                        yield return dto;
                    }

                    if (response.Content.Content.Count < TrendyolDefaults.OrderPageSize)
                        hasMoreOrdersToFetch = false;
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
