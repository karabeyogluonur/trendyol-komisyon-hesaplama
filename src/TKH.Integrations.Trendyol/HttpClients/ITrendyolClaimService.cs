using Refit;
using TKH.Business.Integrations.Providers.Trendyol.Models;

namespace TKH.Integrations.Trendyol.HttpClients
{
    public interface ITrendyolClaimService
    {
        [Get("/integration/order/sellers/{sellerId}/claims")]
        Task<IApiResponse<TrendyolClaimResponse>> GetClaimsAsync(long sellerId, [Query] TrendyolClaimSearchRequest trendyolClaimSearchRequest);
    }
}
