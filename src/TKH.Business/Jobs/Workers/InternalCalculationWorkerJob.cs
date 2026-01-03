using System.ComponentModel;
using Hangfire;
using TKH.Business.Features.ProductExpenses.Services;
using TKH.Business.Features.Products.Services;

namespace TKH.Business.Jobs.Workers
{
    public class InternalCalculationWorkerJob
    {
        private readonly IProductExpenseSyncService _productExpenseSyncService;
        private readonly IProductSyncService _productSyncService;

        public InternalCalculationWorkerJob(IProductExpenseSyncService productExpenseSyncService, IProductSyncService productSyncService)
        {
            _productExpenseSyncService = productExpenseSyncService;
            _productSyncService = productSyncService;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 3600)]
        [AutomaticRetry(Attempts = 0)]
        [DisplayName("Ürün Kargo Gider Analizi ve Güncelleme")]
        public async Task ExecuteProductShippingCostAnalysisAsync()
        {
            await _productExpenseSyncService.CalculateAndSyncShippingCostsAsync();
        }
        [DisableConcurrentExecution(timeoutInSeconds: 3600)]
        [AutomaticRetry(Attempts = 0)]
        [DisplayName("Ürün Komisyon Oranı Senkronizasyonu")]
        public async Task ExecuteProductCommissionSyncAsync()
        {
            await _productSyncService.CalculateAndSyncCommissionRatesAsync();
        }
    }
}
