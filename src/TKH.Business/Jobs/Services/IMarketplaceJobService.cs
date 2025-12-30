using TKH.Entities.Enums;

namespace TKH.Business.Jobs.Services
{
    public interface IMarketplaceJobService
    {
        Task DispatchScheduledAllAccountsDataSyncAsync();
        Task DispatchMarketplaceCategoryDataSyncAsync();
        void DispatchImmediateSingleAccountDataSync(int marketplaceAccountId);
    }
}
