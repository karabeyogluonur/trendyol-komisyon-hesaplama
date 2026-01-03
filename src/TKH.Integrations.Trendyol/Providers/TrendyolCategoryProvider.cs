using AutoMapper;
using Refit;
using TKH.Business.Executors;
using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Business.Integrations.Providers.Trendyol.Models;
using TKH.Entities.Enums;
using TKH.Integrations.Trendyol.HttpClients;
using TKH.Integrations.Trendyol.Infrastructure;
using TKH.Integrations.Trendyol.Policies;

namespace TKH.Integrations.Trendyol.Providers
{
    public class TrendyolCategoryProvider : IMarketplaceCategoryProvider
    {
        private readonly TrendyolClientFactory _trendyolClientFactory;
        private readonly IIntegrationExecutor _integrationExecutor;
        private readonly TrendyolErrorPolicy _trendyolErrorPolicy;
        private readonly IMapper _mapper;

        public TrendyolCategoryProvider(
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

        public async Task<List<MarketplaceCategoryDto>> GetCategoryTreeAsync(CancellationToken cancellationToken = default)
        {
            ITrendyolCategoryService trendyolCategoryService = _trendyolClientFactory.CreatePublicClient<ITrendyolCategoryService>();

            TrendyolCategoryResponse apiResponse = await _integrationExecutor.ExecuteRefitAsync(() => trendyolCategoryService.GetCategoriesAsync(), _trendyolErrorPolicy);

            List<MarketplaceCategoryDto> flatCategoryList = FlattenAndMapCategories(apiResponse.Categories);

            return flatCategoryList;
        }

        public async Task<List<MarketplaceCategoryAttributeDto>> GetCategoryAttributesAsync(string marketplaceCategoryId, CancellationToken cancellationToken = default)
        {
            if (!int.TryParse(marketplaceCategoryId, out int categoryIdInt))
                return new List<MarketplaceCategoryAttributeDto>();

            ITrendyolCategoryService trendyolCategoryService = _trendyolClientFactory.CreatePublicClient<ITrendyolCategoryService>();

            TrendyolCategoryAttributeResponse apiResponse = await _integrationExecutor.ExecuteRefitAsync(() => trendyolCategoryService.GetCategoryAttributesAsync(categoryIdInt), _trendyolErrorPolicy);

            List<MarketplaceCategoryAttributeDto> marketplaceCategoryAttributeDtoList = _mapper.Map<List<MarketplaceCategoryAttributeDto>>(apiResponse.CategoryAttributes);

            return marketplaceCategoryAttributeDtoList;
        }

        private List<MarketplaceCategoryDto> FlattenAndMapCategories(List<TrendyolCategoryContent> sourceCategories)
        {
            List<MarketplaceCategoryDto> resultList = new List<MarketplaceCategoryDto>();

            foreach (TrendyolCategoryContent sourceCategory in sourceCategories)
            {
                MarketplaceCategoryDto mappedCategory = _mapper.Map<MarketplaceCategoryDto>(sourceCategory);
                resultList.Add(mappedCategory);

                if (sourceCategory.SubCategories != null && sourceCategory.SubCategories.Count > 0)
                {
                    List<MarketplaceCategoryDto> subCategories = FlattenAndMapCategories(sourceCategory.SubCategories);
                    resultList.AddRange(subCategories);
                }
            }

            return resultList;
        }
    }
}
