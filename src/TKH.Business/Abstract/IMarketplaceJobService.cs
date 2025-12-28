using TKH.Entities.Enums;

namespace TKH.Business.Abstract
{
    public interface IMarketplaceJobService
    {
        Task DispatchScheduledAllAccountsDataSyncAsync();
        Task DispatchMarketplaceReferenceDataSyncAsync();
        void DispatchImmediateSingleAccountDataSync(int marketplaceAccountId);
    }
}
