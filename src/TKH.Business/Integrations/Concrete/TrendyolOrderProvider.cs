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
                        Status = TrendyolOrderStatus.Delivered,
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
                        await Task.Delay(TrendyolDefaults.ApiRateLimitDelayMs, cancellationToken);
                    }
                }

                currentWindowStartDate = currentWindowEndDate;
            }
        }
    }
}
