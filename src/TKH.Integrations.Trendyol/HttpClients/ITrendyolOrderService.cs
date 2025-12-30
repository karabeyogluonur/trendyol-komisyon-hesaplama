using Refit;
using TKH.Business.Integrations.Providers.Trendyol.Models;

namespace TKH.Integrations.Trendyol.HttpClients
{
    public interface ITrendyolOrderService
    {
        [Get("/integration/order/sellers/{sellerId}/orders")]
        Task<IApiResponse<TrendyolOrderResponse>> GetOrdersAsync(long sellerId, [Query] TrendyolOrderSearchRequest trendyolOrderSearchRequest);
    }
}
