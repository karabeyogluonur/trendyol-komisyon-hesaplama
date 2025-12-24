using Refit;
using TKH.Business.Integrations.Providers.Trendyol.Models;

namespace TKH.Business.Integrations.Providers.Trendyol.Services
{
    public interface ITrendyolOrderService
    {
        [Get("/integration/order/sellers/{sellerId}/orders")]
        Task<IApiResponse<TrendyolOrderResponse>> GetOrdersAsync(long sellerId, [Query] TrendyolOrderSearchRequest trendyolOrderSearchRequest);
    }
}
