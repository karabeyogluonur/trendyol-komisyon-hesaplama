using Refit;
using TKH.Business.Integrations.Providers.Trendyol.Models;

namespace TKH.Integrations.Trendyol.HttpClients
{
    public interface ITrendyolCategoryService
    {
        [Get("/integration/product/product-categories")]
        Task<IApiResponse<TrendyolCategoryResponse>> GetCategoriesAsync();

        [Get("/integration/product/product-categories/{categoryId}/attributes")]
        Task<IApiResponse<TrendyolCategoryAttributeResponse>> GetCategoryAttributesAsync(int categoryId);
    }
}
