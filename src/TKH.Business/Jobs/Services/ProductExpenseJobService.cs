using Hangfire;
using Hangfire.States;
using TKH.Business.Features.MarketplaceAccounts.Services;
using TKH.Business.Jobs.Workers;
using TKH.Core.Utilities.Results;
using TKH.Entities.Enums;

namespace TKH.Business.Jobs.Services
{
    public class ProductExpenseJobService : IProductExpenseJobService
    {
        private readonly IMarketplaceAccountService _marketplaceAccountService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public ProductExpenseJobService(
            IMarketplaceAccountService marketplaceAccountService,
            IBackgroundJobClient backgroundJobClient)
        {
            _marketplaceAccountService = marketplaceAccountService;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task DispatchScheduledAllAccountsExpenseAnalysisAsync()
        {
            IDataResult<IList<int>> activeAccountsResult = await _marketplaceAccountService.GetActiveConnectedAccountIdsAsync();

            if (!activeAccountsResult.Success || activeAccountsResult.Data is null)
                return;

            foreach (int marketplaceAccountId in activeAccountsResult.Data)
            {
                string shippingJobId = _backgroundJobClient.Enqueue<ProductExpenseWorkerJob>(
                    job => job.ExecuteShippingCostAnalysisAsync(marketplaceAccountId));

                _backgroundJobClient.ContinueJobWith<ProductExpenseWorkerJob>(
                    shippingJobId,
                    job => job.ExecuteCommissionRateAnalysisAsync(marketplaceAccountId),
                    JobContinuationOptions.OnlyOnSucceededState);
            }
        }

        public void DispatchImmediateSingleAccountExpenseAnalysis(int marketplaceAccountId)
        {
            string queueName = BackgroundJobQueue.Critical.ToString().ToLowerInvariant();
            EnqueuedState enqueuedState = new EnqueuedState(queueName);

            string shippingJobId = _backgroundJobClient.Create<ProductExpenseWorkerJob>(
                job => job.ExecuteShippingCostAnalysisAsync(marketplaceAccountId),
                enqueuedState);

            _backgroundJobClient.ContinueJobWith<ProductExpenseWorkerJob>(
                shippingJobId,
                job => job.ExecuteCommissionRateAnalysisAsync(marketplaceAccountId),
                JobContinuationOptions.OnlyOnSucceededState);
        }

        public string ContinueWithExpenseAnalysisChain(int marketplaceAccountId, string parentJobId)
        {
            string shippingAnalysisJobId = _backgroundJobClient.ContinueJobWith<ProductExpenseWorkerJob>(
                parentJobId,
                job => job.ExecuteShippingCostAnalysisAsync(marketplaceAccountId),
                JobContinuationOptions.OnlyOnSucceededState);

            string commissionAnalysisJobId = _backgroundJobClient.ContinueJobWith<ProductExpenseWorkerJob>(
                shippingAnalysisJobId,
                job => job.ExecuteCommissionRateAnalysisAsync(marketplaceAccountId),
                JobContinuationOptions.OnlyOnSucceededState);

            return commissionAnalysisJobId;
        }
    }
}
