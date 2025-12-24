using Refit;
using TKH.Business.Integrations.Providers.Trendyol.Models;

namespace TKH.Business.Integrations.Providers.Trendyol.Services
{
    public interface ITrendyolFinanceService
    {
        [Get("/integration/finance/che/sellers/{sellerId}/settlements")]
        Task<IApiResponse<TrendyolFinancialResponse>> GetSettlementsAsync(long sellerId, [Query] TrendyolSettlementSearchRequest request);
        [Get("/integration/finance/che/sellers/{sellerId}/otherfinancials")]
        Task<IApiResponse<TrendyolFinancialResponse>> GetOtherFinancialsAsync(long sellerId, [Query] TrendyolOtherFinancialSearchRequest request);
    }
}
