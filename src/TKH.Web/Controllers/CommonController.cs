using Microsoft.AspNetCore.Mvc;
using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Business.Features.MarketplaceAccounts.Services;
using TKH.Core.Contexts;
using TKH.Core.Utilities.Results;
using TKH.Entities.Enums;
using TKH.Web.Infrastructure.Services;

namespace TKH.Web.Controllers
{
    public class CommonController : BaseController
    {
        private readonly IWorkContext _workContext;
        private readonly INotificationService _notificationService;
        private readonly IMarketplaceAccountService _marketplaceAccountService;

        public CommonController(
            IWorkContext workContext,
            INotificationService notificationService,
            IMarketplaceAccountService marketplaceAccountService)
        {
            _workContext = workContext;
            _notificationService = notificationService;
            _marketplaceAccountService = marketplaceAccountService;
        }

        [HttpPost]
        public async Task<IActionResult> SetCurrentMarketplaceAccount(int accountId, string returnUrl)
        {
            if (_workContext.CurrentMarketplaceAccountId == accountId)
                return RedirectToLocal(returnUrl);

            IDataResult<MarketplaceAccountDetailsDto> marketplaceAccountResult = await _marketplaceAccountService.GetByIdAsync(accountId);

            if (!marketplaceAccountResult.Success || marketplaceAccountResult.Data is null)
            {
                _notificationService.Error("Seçilen mağaza sistemde bulunamadı.");
                return RedirectToLocal(returnUrl);
            }

            MarketplaceAccountDetailsDto marketplaceAccount = marketplaceAccountResult.Data;

            if (marketplaceAccount.SyncState == MarketplaceSyncState.Syncing ||
                marketplaceAccount.SyncState == MarketplaceSyncState.Queued)
            {
                _notificationService.Warning("Bu mağaza şu anda işlem gördüğü (Veri Çekme/Sıra) için seçilemez.");
                return RedirectToLocal(returnUrl);
            }

            if (marketplaceAccount.ConnectionState == MarketplaceConnectionState.Initializing ||
                marketplaceAccount.ConnectionState == MarketplaceConnectionState.AuthError ||
                marketplaceAccount.ConnectionState == MarketplaceConnectionState.SystemError)
            {
                _notificationService.Error("Bağlantı sorunu olan veya kurulumu tamamlanmamış mağazaya geçiş yapılamaz. Lütfen yönetim panelinden düzeltin.");
                return RedirectToLocal(returnUrl);
            }

            _workContext.CurrentMarketplaceAccountId = accountId;

            _notificationService.Info($"Aktif mağaza değiştirildi: {marketplaceAccount.StoreName}");

            return RedirectToLocal(returnUrl);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
