using AutoMapper;
using System.Runtime.CompilerServices;
using TKH.Business.Integrations.Abstract;
using TKH.Business.Integrations.Dtos;
using TKH.Business.Integrations.Providers.Trendyol;
using TKH.Business.Integrations.Providers.Trendyol.Models;
using TKH.Business.Integrations.Providers.Trendyol.Services;
using Refit;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Entities.Enums;
using TKH.Core.Common.Constants;

namespace TKH.Business.Integrations.Concrete
{
    public class TrendyolProductProvider : IMarketplaceProductProvider
    {
        private readonly TrendyolClientFactory _trendyolClientFactory;
        private readonly IMapper _mapper;

        public TrendyolProductProvider(TrendyolClientFactory trendyolClientFactory, IMapper mapper)
        {
            _trendyolClientFactory = trendyolClientFactory;
            _mapper = mapper;
        }

        public async IAsyncEnumerable<MarketplaceProductDto> GetProductsStreamAsync(
            MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (!long.TryParse(marketplaceAccountConnectionDetailsDto.MerchantId, out long sellerIdentifier))
                yield break;

            ITrendyolProductService trendyolProductApi = _trendyolClientFactory.CreateClient<ITrendyolProductService>(
                sellerIdentifier,
                marketplaceAccountConnectionDetailsDto.ApiKey,
                marketplaceAccountConnectionDetailsDto.ApiSecretKey);

            int currentPageIndex = 0;
            bool hasMoreProductsToFetch = true;

            while (hasMoreProductsToFetch && !cancellationToken.IsCancellationRequested)
            {
                TrendyolProductSearchRequest trendyolFilterGetProducts = new TrendyolProductSearchRequest
                {
                    Page = currentPageIndex,
                    Size = TrendyolDefaults.ProductPageSize,
                    Approved = true
                };

                IApiResponse<TrendyolProductResponse> apiResponse = await trendyolProductApi.GetProductsAsync(
                    sellerIdentifier,
                    trendyolFilterGetProducts);

                if (!apiResponse.IsSuccessStatusCode || apiResponse.Content?.Content is null || apiResponse.Content.Content.Count == 0)
                    yield break;

                foreach (TrendyolProductContent productItem in apiResponse.Content.Content)
                {
                    MarketplaceProductDto marketplaceProductDto = _mapper.Map<MarketplaceProductDto>(productItem);
                    EnrichProductWithExpenses(marketplaceProductDto);
                    yield return marketplaceProductDto;
                }

                if (apiResponse.Content.Content.Count < TrendyolDefaults.ProductPageSize)
                    hasMoreProductsToFetch = false;
                else
                {
                    currentPageIndex++;
                    await Task.Delay(TrendyolDefaults.ApiRateLimitDelayMs, cancellationToken);
                }
            }
        }

        private void EnrichProductWithExpenses(MarketplaceProductDto marketplaceProductDto)
        {
            MarketplaceProductExpenseDto trendyolServiceFeeExpenseDto = new MarketplaceProductExpenseDto
            {
                Type = ProductExpenseType.MarketplaceServiceFee,
                Amount = TrendyolDefaults.FixedServiceFeeAmount,
                VatRate = FinancialConstants.StandardServiceVatRate,
                IsVatIncluded = TrendyolDefaults.IsFixedServiceFeeVatIncluded
            };

            marketplaceProductDto.Expenses.Add(trendyolServiceFeeExpenseDto);
        }
    }
}
