using Refit;
using TKH.Business.Integrations.Providers.Trendyol.Models;

namespace TKH.Business.Integrations.Providers.Trendyol.Services
{
    public interface ITrendyolReferenceService
    {
        [Get("/integration/product/product-categories")]
        Task<IApiResponse<TrendyolCategoryResponse>> GetCategoriesAsync();

        [Get("/integration/product/product-categories/{categoryId}/attributes")]
        Task<IApiResponse<TrendyolCategoryAttributeResponse>> GetCategoryAttributesAsync(int categoryId);
    }
}
