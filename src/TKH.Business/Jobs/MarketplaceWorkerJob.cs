using System.ComponentModel;
using Hangfire;
using TKH.Business.Abstract;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Business.Jobs.Filters;
using TKH.Entities.Enums;

namespace TKH.Business.Jobs
{
    public class MarketplaceWorkerJob
    {
        private readonly IMarketplaceAccountService _marketplaceAccountService;
        private readonly IProductSyncService _productSyncService;
        private readonly IOrderSyncService _orderSyncService;
        private readonly IClaimSyncService _claimSyncService;
        private readonly IFinanceSyncService _financeSyncService;
        private readonly IMarketplaceReferenceSyncService _marketplaceReferenceSyncService;

        public MarketplaceWorkerJob(
            IMarketplaceAccountService marketplaceAccountService,
            IProductSyncService productSyncService,
            IOrderSyncService orderSyncService,
            IClaimSyncService claimSyncService,
            IFinanceSyncService financeSyncService,
            IMarketplaceReferenceSyncService marketplaceReferenceSyncService)
        {
            _marketplaceAccountService = marketplaceAccountService;
            _productSyncService = productSyncService;
            _orderSyncService = orderSyncService;
            _claimSyncService = claimSyncService;
            _financeSyncService = financeSyncService;
            _marketplaceReferenceSyncService = marketplaceReferenceSyncService;
        }

        private async Task<MarketplaceAccountConnectionDetailsDto> GetConnectionDetailsOrThrow(int accountId)
        {
            var result = await _marketplaceAccountService.GetConnectionDetailsAsync(accountId);

            if (!result.Success || result.Data == null)
                throw new Exception($"Hesap bağlantı detayları alınamadı. AccountID: {accountId} - Hata: {result.Message}");

            return result.Data;
        }

        [MarketplaceJobState]
        [DisableConcurrentExecution(timeoutInSeconds: 1800)]
        [DisplayName("1. Ürün Senkronizasyonu | Hesap: {0}")]
        public async Task SyncProductsStep(int accountId)
        {
            var connectionDetails = await GetConnectionDetailsOrThrow(accountId);
            await _productSyncService.SyncProductsFromMarketplaceAsync(connectionDetails);
        }

        [MarketplaceJobState]
        [DisableConcurrentExecution(timeoutInSeconds: 1800)]
        [DisplayName("2. Sipariş Senkronizasyonu | Hesap: {0}")]
        public async Task SyncOrdersStep(int accountId)
        {
            var connectionDetails = await GetConnectionDetailsOrThrow(accountId);
            await _orderSyncService.SyncOrdersFromMarketplaceAsync(connectionDetails);
        }

        [MarketplaceJobState]
        [DisableConcurrentExecution(timeoutInSeconds: 1800)]
        [DisplayName("3. İade Senkronizasyonu | Hesap: {0}")]
        public async Task SyncClaimsStep(int accountId)
        {
            var connectionDetails = await GetConnectionDetailsOrThrow(accountId);
            await _claimSyncService.SyncClaimsFromMarketplaceAsync(connectionDetails);
        }

        [MarketplaceJobState]
        [DisableConcurrentExecution(timeoutInSeconds: 1800)]
        [DisplayName("4. Finans Senkronizasyonu | Hesap: {0}")]
        public async Task SyncFinanceStep(int accountId)
        {
            var connectionDetails = await GetConnectionDetailsOrThrow(accountId);

            await _financeSyncService.SyncFinancialTransactionsFromMarketplaceAsync(connectionDetails);
            await _financeSyncService.SyncShipmentTransactionsFromMarketplaceAsync(connectionDetails);
        }

        [DisableConcurrentExecution(timeoutInSeconds: 1800)]
        [DisplayName("Referans Veri Güncelleme | Tip: {0}")]
        public async Task ExecuteReferenceSyncAsync(MarketplaceType marketplaceType)
        {
            await _marketplaceReferenceSyncService.SyncCategoriesAsync(marketplaceType);
            await _marketplaceReferenceSyncService.SyncCategoryAttributesAsync(marketplaceType);
        }
    }
}
