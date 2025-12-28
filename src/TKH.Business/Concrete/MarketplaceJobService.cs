using Hangfire;
using Hangfire.States;
using TKH.Business.Abstract;
using TKH.Business.Jobs;
using TKH.Core.Utilities.Results;
using TKH.Entities.Enums;

namespace TKH.Business.Concrete
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

            foreach (int accountId in activeAccountsResult.Data)
            {
                string productSyncJobId = _backgroundJobClient.Enqueue<MarketplaceWorkerJob>(
                    job => job.SyncProductsStep(accountId));

                string orderSyncJobId = _backgroundJobClient.ContinueJobWith<MarketplaceWorkerJob>(
                    productSyncJobId,
                    job => job.SyncOrdersStep(accountId),
                    JobContinuationOptions.OnlyOnSucceededState);

                string claimSyncJobId = _backgroundJobClient.ContinueJobWith<MarketplaceWorkerJob>(
                    orderSyncJobId,
                    job => job.SyncClaimsStep(accountId),
                    JobContinuationOptions.OnlyOnSucceededState);

                _backgroundJobClient.ContinueJobWith<MarketplaceWorkerJob>(
                    claimSyncJobId,
                    job => job.SyncFinanceStep(accountId),
                    JobContinuationOptions.OnlyOnSucceededState);
            }
        }

        public async Task DispatchMarketplaceReferenceDataSyncAsync()
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

        public void DispatchImmediateSingleAccountDataSync(int marketplaceAccountId)
        {
            string queueName = BackgroundJobQueue.Critical.ToString().ToLowerInvariant();
            EnqueuedState enqueuedState = new EnqueuedState(queueName);

            string productJobId = _backgroundJobClient.Create<MarketplaceWorkerJob>(
                job => job.SyncProductsStep(marketplaceAccountId),
                enqueuedState);

            string orderJobId = _backgroundJobClient.ContinueJobWith<MarketplaceWorkerJob>(
                productJobId,
                job => job.SyncOrdersStep(marketplaceAccountId),
                JobContinuationOptions.OnlyOnSucceededState);

            string claimJobId = _backgroundJobClient.ContinueJobWith<MarketplaceWorkerJob>(
                orderJobId,
                job => job.SyncClaimsStep(marketplaceAccountId),
                JobContinuationOptions.OnlyOnSucceededState);

            _backgroundJobClient.ContinueJobWith<MarketplaceWorkerJob>(
                claimJobId,
                job => job.SyncFinanceStep(marketplaceAccountId),
                JobContinuationOptions.OnlyOnSucceededState);
        }
    }
}
