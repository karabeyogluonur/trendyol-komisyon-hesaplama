using Refit;
using TKH.Business.Integrations.Providers.Trendyol.Models;

namespace TKH.Integrations.Trendyol.HttpClients
{
    public interface ITrendyolFinanceService
    {
        [Get("/integration/finance/che/sellers/{sellerId}/settlements")]
        Task<IApiResponse<TrendyolFinancialResponse>> GetSettlementsAsync(long sellerId, [Query] TrendyolSettlementSearchRequest request);

        [Get("/integration/finance/che/sellers/{sellerId}/otherfinancials")]
        Task<IApiResponse<TrendyolFinancialResponse>> GetOtherFinancialsAsync(long sellerId, [Query] TrendyolOtherFinancialSearchRequest request);

        [Get("/integration/finance/che/sellers/{sellerId}/cargo-invoice/{invoiceSerialNumber}/items")]
        Task<IApiResponse<TrendyolCargoInvoiceResponse>> GetCargoInvoiceAsync(long sellerId, string invoiceSerialNumber);
    }
}
