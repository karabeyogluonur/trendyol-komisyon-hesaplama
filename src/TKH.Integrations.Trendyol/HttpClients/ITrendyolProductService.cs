using Refit;
using TKH.Business.Integrations.Providers.Trendyol.Models;

namespace TKH.Integrations.Trendyol.HttpClients
{
    public interface ITrendyolProductService
    {
        [Get("/integration/product/sellers/{sellerId}/products")]
        Task<IApiResponse<TrendyolProductResponse>> GetProductsAsync(long sellerId, [Query] TrendyolProductSearchRequest filter);
    }
}
