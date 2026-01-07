using TKH.Business.Features.MarketplaceAccounts.Dtos;

namespace TKH.Business.Features.ProductExpenses.Services
{
    public interface IProductExpenseSyncService
    {
        Task CalculateAndSyncShippingCostsAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto);
        Task CalculateAndSyncCommissionRatesAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto);
    }
}
