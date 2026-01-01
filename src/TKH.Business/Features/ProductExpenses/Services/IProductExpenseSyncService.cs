namespace TKH.Business.Features.ProductExpenses.Services
{
    public interface IProductExpenseSyncService
    {
        Task CalculateAndSyncShippingCostsAsync();
    }
}
