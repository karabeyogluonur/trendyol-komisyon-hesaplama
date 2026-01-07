using System.ComponentModel;
using Hangfire;
using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Business.Features.MarketplaceAccounts.Services;
using TKH.Business.Features.ProductExpenses.Services;
using TKH.Core.Utilities.Results;

namespace TKH.Business.Jobs.Workers
{
    public class ProductExpenseWorkerJob
    {
        private readonly IProductExpenseSyncService _productExpenseSyncService;
        private readonly IMarketplaceAccountService _marketplaceAccountService;

        public ProductExpenseWorkerJob(
            IProductExpenseSyncService productExpenseSyncService,
            IMarketplaceAccountService marketplaceAccountService)
        {
            _productExpenseSyncService = productExpenseSyncService;
            _marketplaceAccountService = marketplaceAccountService;
        }

        private async Task<MarketplaceAccountConnectionDetailsDto> GetConnectionDetailsOrThrow(int marketplaceAccountId)
        {
            IDataResult<MarketplaceAccountConnectionDetailsDto> result = await _marketplaceAccountService.GetConnectionDetailsAsync(marketplaceAccountId);

            if (!result.Success || result.Data is null)
                throw new Exception($"Failed to retrieve account connection details. AccountID: {marketplaceAccountId} - Error: {result.Message}");

            return result.Data;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 3600)]
        [AutomaticRetry(Attempts = 0)]
        [DisplayName("Product Shipping Cost Analysis | Account: {0}")]
        public async Task ExecuteShippingCostAnalysisAsync(int marketplaceAccountId)
        {
            MarketplaceAccountConnectionDetailsDto accountDetails = await GetConnectionDetailsOrThrow(marketplaceAccountId);
            await _productExpenseSyncService.CalculateAndSyncShippingCostsAsync(accountDetails);
        }

        [DisableConcurrentExecution(timeoutInSeconds: 3600)]
        [AutomaticRetry(Attempts = 0)]
        [DisplayName("Product Commission Rate Analysis | Account: {0}")]
        public async Task ExecuteCommissionRateAnalysisAsync(int marketplaceAccountId)
        {
            MarketplaceAccountConnectionDetailsDto accountDetails = await GetConnectionDetailsOrThrow(marketplaceAccountId);
            await _productExpenseSyncService.CalculateAndSyncCommissionRatesAsync(accountDetails);
        }
    }
}
