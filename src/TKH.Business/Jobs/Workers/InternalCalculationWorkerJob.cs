using System.ComponentModel;
using Hangfire;
using TKH.Business.Features.ProductExpenses.Services;

namespace TKH.Business.Jobs.Workers
{
    public class InternalCalculationWorkerJob
    {
        private readonly IProductExpenseSyncService _productExpenseSyncService;

        public InternalCalculationWorkerJob(IProductExpenseSyncService productExpenseSyncService)
        {
            _productExpenseSyncService = productExpenseSyncService;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 3600)]
        [AutomaticRetry(Attempts = 0)]
        [DisplayName("Ürün Kargo Gider Analizi ve Güncelleme")]
        public async Task ExecuteProductShippingCostAnalysisAsync()
        {
            await _productExpenseSyncService.CalculateAndSyncShippingCostsAsync();
        }
    }
}
