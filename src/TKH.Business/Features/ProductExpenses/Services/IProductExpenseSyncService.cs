namespace TKH.Business.Features.ProductExpenses.Services
{
    public interface IProductExpenseSyncService
    {
        Task CalculateAndSyncShippingCostsAsync();
        Task CalculateAndSyncCommissionRatesAsync();
    }
}
