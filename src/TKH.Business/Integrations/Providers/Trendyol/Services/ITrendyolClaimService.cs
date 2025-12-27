using Refit;
using TKH.Business.Integrations.Providers.Trendyol.Models;

namespace TKH.Business.Integrations.Providers.Trendyol.Services
{
    public interface ITrendyolClaimService
    {
        [Get("/integration/order/sellers/{sellerId}/claims")]
        Task<IApiResponse<TrendyolClaimResponse>> GetClaimsAsync(long sellerId, [Query] TrendyolClaimSearchRequest trendyolClaimSearchRequest);
    }
}
