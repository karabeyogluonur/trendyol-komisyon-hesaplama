using TKH.Business.Features.MarketplaceAccounts.Dtos;


namespace TKH.Business.Features.Products.Services
{
    public interface IProductSyncService
    {
        Task SyncProductsFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto);
    }
}
