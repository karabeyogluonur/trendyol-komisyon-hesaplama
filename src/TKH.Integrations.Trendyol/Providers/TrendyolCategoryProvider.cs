using AutoMapper;
using Refit;
using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Business.Integrations.Providers.Trendyol.Models;
using TKH.Entities.Enums;
using TKH.Integrations.Trendyol.HttpClients;
using TKH.Integrations.Trendyol.Infrastructure;

namespace TKH.Integrations.Trendyol.Providers
{
    public class TrendyolCategoryProvider : IMarketplaceCategoryProvider
    {
        private readonly TrendyolClientFactory _trendyolClientFactory;
        private readonly IMapper _mapper;

        public TrendyolCategoryProvider(
            TrendyolClientFactory trendyolClientFactory,
            IMapper mapper)
        {
            _trendyolClientFactory = trendyolClientFactory;
            _mapper = mapper;
        }

        public MarketplaceType MarketplaceType => MarketplaceType.Trendyol;

        public async Task<List<MarketplaceCategoryDto>> GetCategoryTreeAsync(CancellationToken cancellationToken = default)
        {
            ITrendyolReferenceService trendyolReferenceService = _trendyolClientFactory.CreatePublicClient<ITrendyolReferenceService>();

            IApiResponse<TrendyolCategoryResponse> apiResponse = await trendyolReferenceService.GetCategoriesAsync();

            if (!apiResponse.IsSuccessStatusCode || apiResponse.Content == null || apiResponse.Content.Categories == null)
                return new List<MarketplaceCategoryDto>();

            List<MarketplaceCategoryDto> flatCategoryList = FlattenAndMapCategories(apiResponse.Content.Categories);

            return flatCategoryList;
        }

        public async Task<List<MarketplaceCategoryAttributeDto>> GetCategoryAttributesAsync(string marketplaceCategoryId, CancellationToken cancellationToken = default)
        {
            if (!int.TryParse(marketplaceCategoryId, out int categoryIdInt))
                return new List<MarketplaceCategoryAttributeDto>();

            ITrendyolReferenceService trendyolReferenceService = _trendyolClientFactory.CreatePublicClient<ITrendyolReferenceService>();

            IApiResponse<TrendyolCategoryAttributeResponse> apiResponse = await trendyolReferenceService.GetCategoryAttributesAsync(categoryIdInt);

            if (!apiResponse.IsSuccessStatusCode || apiResponse.Content == null || apiResponse.Content.CategoryAttributes == null)
                return new List<MarketplaceCategoryAttributeDto>();

            List<MarketplaceCategoryAttributeDto> marketplaceCategoryAttributeDtoList = _mapper.Map<List<MarketplaceCategoryAttributeDto>>(apiResponse.Content.CategoryAttributes);

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
