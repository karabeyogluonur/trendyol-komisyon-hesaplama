using AutoMapper;
using System.Runtime.CompilerServices;
using TKH.Business.Integrations.Providers.Trendyol;
using TKH.Business.Integrations.Providers.Trendyol.Models;
using Refit;
using TKH.Entities.Enums;
using TKH.Core.Common.Constants;
using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Integrations.Trendyol.Infrastructure;
using TKH.Integrations.Trendyol.HttpClients;
using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Business.Executors;
using TKH.Integrations.Trendyol.Policies;

namespace TKH.Integrations.Trendyol.Providers
{
    public class TrendyolProductProvider : IMarketplaceProductProvider
    {
        private readonly TrendyolClientFactory _trendyolClientFactory;
        private readonly IIntegrationExecutor _integrationExecutor;
        private readonly TrendyolErrorPolicy _trendyolErrorPolicy;
        private readonly IMapper _mapper;

        public TrendyolProductProvider(
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
                };

                TrendyolProductResponse apiResponse = await _integrationExecutor.ExecuteRefitAsync(() => trendyolProductApi.GetProductsAsync(sellerIdentifier, trendyolFilterGetProducts), _trendyolErrorPolicy);

                if (apiResponse.Content is null || apiResponse.Content.Count is 0)
                    yield break;

                foreach (TrendyolProductContent productItem in apiResponse.Content)
                {
                    MarketplaceProductDto marketplaceProductDto = _mapper.Map<MarketplaceProductDto>(productItem);
                    EnrichProductWithExpenses(marketplaceProductDto);
                    yield return marketplaceProductDto;
                }

                if (apiResponse.Content.Count < TrendyolDefaults.ProductPageSize)
                    hasMoreProductsToFetch = false;
                else
                    currentPageIndex++;
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
