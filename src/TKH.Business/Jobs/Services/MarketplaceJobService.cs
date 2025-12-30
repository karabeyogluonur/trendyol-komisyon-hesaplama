using Hangfire;
using Hangfire.States;
using TKH.Business.Features.MarketplaceAccounts.Services;
using TKH.Business.Jobs.Workers;
using TKH.Core.Utilities.Results;
using TKH.Entities.Enums;

namespace TKH.Business.Jobs.Services
{
    public class MarketplaceJobService : IMarketplaceJobService
    {
        private readonly IMarketplaceAccountService _marketplaceAccountService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public MarketplaceJobService(
            IMarketplaceAccountService marketplaceAccountService,
            IBackgroundJobClient backgroundJobClient)
        {
            _marketplaceAccountService = marketplaceAccountService;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task DispatchScheduledAllAccountsDataSyncAsync()
        {
            IDataResult<IList<int>> activeAccountsResult = await _marketplaceAccountService.GetActiveConnectedAccountIdsAsync();

            if (!activeAccountsResult.Success || activeAccountsResult.Data == null)
                return;

            foreach (int marketplaceAccountId in activeAccountsResult.Data)
                await ExecuteAccountSyncChainAsync(marketplaceAccountId, null);
        }

        public void DispatchImmediateSingleAccountDataSync(int marketplaceAccountId)
        {
            string queueName = BackgroundJobQueue.Critical.ToString().ToLowerInvariant();
            EnqueuedState enqueuedState = new EnqueuedState(queueName);

            ExecuteAccountSyncChainAsync(marketplaceAccountId, enqueuedState).GetAwaiter().GetResult();
        }

        public async Task DispatchMarketplaceCategoryDataSyncAsync()
        {
            MarketplaceType[] marketplaceTypes = Enum.GetValues<MarketplaceType>();
            int delayMinutes = 0;

            foreach (MarketplaceType marketplaceType in marketplaceTypes)
            {
                _backgroundJobClient.Schedule<MarketplaceWorkerJob>(
                    job => job.ExecuteReferenceSyncAsync(marketplaceType),
                    TimeSpan.FromMinutes(delayMinutes)
                );

                delayMinutes += 60;
            }

            await Task.CompletedTask;
        }

        private async Task ExecuteAccountSyncChainAsync(int marketplaceAccountId, EnqueuedState? enqueuedState)
        {
            bool isLockAcquired = await _marketplaceAccountService.TryMarkAsSyncingAsync(marketplaceAccountId);

            if (!isLockAcquired)
                return;

            try
            {
                string productSyncJobId;

                if (enqueuedState is not null)
                    productSyncJobId = _backgroundJobClient.Create<MarketplaceWorkerJob>(job => job.SyncProductsStep(marketplaceAccountId), enqueuedState);
                else
                    productSyncJobId = _backgroundJobClient.Enqueue<MarketplaceWorkerJob>(job => job.SyncProductsStep(marketplaceAccountId));

                string orderSyncJobId = _backgroundJobClient.ContinueJobWith<MarketplaceWorkerJob>(
                    productSyncJobId,
                    job => job.SyncOrdersStep(marketplaceAccountId),
                    JobContinuationOptions.OnlyOnSucceededState);

                string claimSyncJobId = _backgroundJobClient.ContinueJobWith<MarketplaceWorkerJob>(
                    orderSyncJobId,
                    job => job.SyncClaimsStep(marketplaceAccountId),
                    JobContinuationOptions.OnlyOnSucceededState);

                string financeSyncJobId = _backgroundJobClient.ContinueJobWith<MarketplaceWorkerJob>(
                    claimSyncJobId,
                    job => job.SyncFinanceStep(marketplaceAccountId),
                    JobContinuationOptions.OnlyOnSucceededState);

                _backgroundJobClient.ContinueJobWith<MarketplaceWorkerJob>(
                    financeSyncJobId,
                    job => job.FinalizeSyncStep(marketplaceAccountId),
                    JobContinuationOptions.OnlyOnSucceededState);
            }
            catch (Exception ex)
            {
                await _marketplaceAccountService.MarkSyncFailedAsync(marketplaceAccountId, ex);
            }
        }
    }
}
