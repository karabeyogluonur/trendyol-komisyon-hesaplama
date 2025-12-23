using Refit;
using TKH.Business.Integrations.Providers.Trendyol.Models;

namespace TKH.Business.Integrations.Providers.Trendyol.Services
{
    public interface ITrendyolProductService
    {
        [Get("/integration/product/sellers/{sellerId}/products")]
        Task<IApiResponse<TrendyolResponseGetProducts>> GetProductsAsync(long sellerId, [Query] TrendyolFilterGetProducts filter);
    }
}
