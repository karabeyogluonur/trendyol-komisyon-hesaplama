using AutoMapper;
using System.Runtime.CompilerServices;
using TKH.Business.Integrations.Providers.Trendyol;
using TKH.Business.Integrations.Providers.Trendyol.Models;
using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Integrations.Trendyol.Infrastructure;
using TKH.Integrations.Trendyol.HttpClients;
using TKH.Integrations.Trendyol.Enums;
using TKH.Entities.Enums;
using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Business.Executors;
using TKH.Integrations.Trendyol.Policies;

namespace TKH.Integrations.Trendyol.Providers
{
    public class TrendyolOrderProvider : IMarketplaceOrderProvider
    {
        private readonly TrendyolClientFactory _trendyolClientFactory;
        private readonly IIntegrationExecutor _integrationExecutor;
        private readonly TrendyolErrorPolicy _trendyolErrorPolicy;
        private readonly IMapper _mapper;

        public TrendyolOrderProvider(
            TrendyolClientFactory trendyolClientFactory,
            IIntegrationExecutor integrationExecutor,
            TrendyolErrorPolicy trendyolErrorPolicy,
            IMapper mapper)
        {
            _trendyolClientFactory = trendyolClientFactory;
            _trendyolErrorPolicy = trendyolErrorPolicy;
            _integrationExecutor = integrationExecutor;
            _mapper = mapper;
        }

        public MarketplaceType MarketplaceType => MarketplaceType.Trendyol;

        public async IAsyncEnumerable<MarketplaceOrderDto> GetOrdersStreamAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (!long.TryParse(marketplaceAccountConnectionDetailsDto.MerchantId, out long sellerIdentifier))
                yield break;

            ITrendyolOrderService trendyolOrderApi = _trendyolClientFactory.CreateClient<ITrendyolOrderService>(sellerIdentifier, marketplaceAccountConnectionDetailsDto.ApiKey, marketplaceAccountConnectionDetailsDto.ApiSecretKey);


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

                    TrendyolOrderResponse response = await _integrationExecutor.ExecuteRefitAsync(() => trendyolOrderApi.GetOrdersAsync(sellerIdentifier, request), _trendyolErrorPolicy);


                    if (response.Content is null || response.Content.Count is 0)
                    {
                        hasMoreOrdersToFetch = false;
                        continue;
                    }

                    foreach (TrendyolOrderContent order in response.Content)
                    {
                        MarketplaceOrderDto dto = _mapper.Map<MarketplaceOrderDto>(order);
                        dto.MarketplaceAccountId = marketplaceAccountConnectionDetailsDto.Id;

                        yield return dto;
                    }

                    if (response.Content.Count < TrendyolDefaults.OrderPageSize)
                        hasMoreOrdersToFetch = false;
                    else
                        currentPageIndex++;
                }

                currentWindowStartDate = currentWindowEndDate;
            }
        }
    }
}
