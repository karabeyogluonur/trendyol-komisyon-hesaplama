using TKH.Entities.Enums;

namespace TKH.Business.Jobs.Services
{
    public interface IMarketplaceJobService
    {
        Task DispatchScheduledAllAccountsDataSyncAsync();
        Task DispatchMarketplaceReferenceDataSyncAsync();
        void DispatchImmediateSingleAccountDataSync(int marketplaceAccountId);
    }
}
