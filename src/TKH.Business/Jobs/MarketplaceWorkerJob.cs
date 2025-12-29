using System.ComponentModel;
using Hangfire;
using TKH.Business.Abstract;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Core.Utilities.Results;
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

        private async Task<MarketplaceAccountConnectionDetailsDto> GetConnectionDetailsOrThrow(int marketplaceAccountId)
        {
            IDataResult<MarketplaceAccountConnectionDetailsDto> marketplaceAccountConnectionDetailsResult = await _marketplaceAccountService.GetConnectionDetailsAsync(marketplaceAccountId);

            if (!marketplaceAccountConnectionDetailsResult.Success || marketplaceAccountConnectionDetailsResult.Data is null)
                throw new Exception($"Hesap bağlantı detayları alınamadı. AccountID: {marketplaceAccountId} - Hata: {marketplaceAccountConnectionDetailsResult.Message}");

            return marketplaceAccountConnectionDetailsResult.Data;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 1800)]
        [AutomaticRetry(Attempts = 0)]
        [DisplayName("1. Ürün Senkronizasyonu | Hesap: {0}")]
        public async Task SyncProductsStep(int marketplaceAccountId)
        {
            MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto = await GetConnectionDetailsOrThrow(marketplaceAccountId);
            await _productSyncService.SyncProductsFromMarketplaceAsync(marketplaceAccountConnectionDetailsDto);
        }

        [DisableConcurrentExecution(timeoutInSeconds: 1800)]
        [AutomaticRetry(Attempts = 0)]
        [DisplayName("2. Sipariş Senkronizasyonu | Hesap: {0}")]
        public async Task SyncOrdersStep(int marketplaceAccountId)
        {
            MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto = await GetConnectionDetailsOrThrow(marketplaceAccountId);
            await _orderSyncService.SyncOrdersFromMarketplaceAsync(marketplaceAccountConnectionDetailsDto);
        }

        [DisableConcurrentExecution(timeoutInSeconds: 1800)]
        [AutomaticRetry(Attempts = 0)]
        [DisplayName("3. İade Senkronizasyonu | Hesap: {0}")]
        public async Task SyncClaimsStep(int marketplaceAccountId)
        {
            MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto = await GetConnectionDetailsOrThrow(marketplaceAccountId);
            await _claimSyncService.SyncClaimsFromMarketplaceAsync(marketplaceAccountConnectionDetailsDto);
        }

        [AutomaticRetry(Attempts = 0)]
        [DisableConcurrentExecution(timeoutInSeconds: 1800)]
        [DisplayName("4. Finans Senkronizasyonu | Hesap: {0}")]
        public async Task SyncFinanceStep(int marketplaceAccountId)
        {
            MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto = await GetConnectionDetailsOrThrow(marketplaceAccountId);

            await _financeSyncService.SyncFinancialTransactionsFromMarketplaceAsync(marketplaceAccountConnectionDetailsDto);
            await _financeSyncService.SyncShipmentTransactionsFromMarketplaceAsync(marketplaceAccountConnectionDetailsDto);
        }

        [AutomaticRetry(Attempts = 3)]
        [DisableConcurrentExecution(timeoutInSeconds: 1800)]
        [DisplayName("Senkronizasyon Bitiş | Hesap: {0}")]
        public async Task FinalizeSyncStep(int marketplaceAccountId)
        {
            await _marketplaceAccountService.MarkSyncCompletedAsync(marketplaceAccountId);
        }

        [DisableConcurrentExecution(timeoutInSeconds: 1800)]
        [AutomaticRetry(Attempts = 0)]
        [DisplayName("Referans Veri Güncelleme | Tip: {0}")]
        public async Task ExecuteReferenceSyncAsync(MarketplaceType marketplaceType)
        {
            await _marketplaceReferenceSyncService.SyncCategoriesAsync(marketplaceType);
            await _marketplaceReferenceSyncService.SyncCategoryAttributesAsync(marketplaceType);
        }
    }
}
