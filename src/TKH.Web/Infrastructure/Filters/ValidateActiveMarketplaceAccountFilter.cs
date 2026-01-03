using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Business.Features.MarketplaceAccounts.Services;
using TKH.Core.Contexts;
using TKH.Core.Utilities.Results;
using TKH.Entities.Enums;
using TKH.Web.Infrastructure.Services;

namespace TKH.Web.Infrastructure.Filters
{
    public class ValidateActiveMarketplaceAccountFilter : IAsyncActionFilter
    {
        private readonly IWorkContext _workContext;
        private readonly IMarketplaceAccountService _marketplaceAccountService;
        private readonly INotificationService _notificationService;

        public ValidateActiveMarketplaceAccountFilter(
            IWorkContext workContext,
            IMarketplaceAccountService marketplaceAccountService,
            INotificationService notificationService)
        {
            _workContext = workContext;
            _marketplaceAccountService = marketplaceAccountService;
            _notificationService = notificationService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!_workContext.CurrentMarketplaceAccountId.HasValue || _workContext.CurrentMarketplaceAccountId.Value <= 0)
            {
                await next();
                return;
            }

            int accountId = _workContext.CurrentMarketplaceAccountId.Value;

            IDataResult<MarketplaceAccountDetailsDto> getMarketplaceAccountResult = await _marketplaceAccountService.GetByIdAsync(accountId);

            if (!getMarketplaceAccountResult.Success || getMarketplaceAccountResult.Data == null)
            {
                await ResetSelectionAndRedirectAsync(context, "Seçili mağaza bulunamadı.");
                return;
            }

            MarketplaceAccountDetailsDto marketplaceAccountDetailsDto = getMarketplaceAccountResult.Data;

            if (marketplaceAccountDetailsDto.SyncState is MarketplaceSyncState.Syncing)
            {
                await ResetSelectionAndRedirectAsync(context, $"'{marketplaceAccountDetailsDto.StoreName}' mağazası güncelleme işlemine girdiği için kısa bir süre beklemeniz gerekmektedir.");
                return;
            }

            if (marketplaceAccountDetailsDto.ConnectionState is not MarketplaceConnectionState.Connected)
            {
                await ResetSelectionAndRedirectAsync(context, $"'{marketplaceAccountDetailsDto.StoreName}' mağazasındaki bağlantı sorunu nedeniyle seçiminiz kaldırıldı.");
                return;
            }

            await next();
        }

        private Task ResetSelectionAndRedirectAsync(ActionExecutingContext context, string message)
        {
            _workContext.CurrentMarketplaceAccountId = null;
            _notificationService.Warning(message);
            context.Result = new RedirectToActionResult("Index", "Home", null);
            return Task.CompletedTask;
        }
    }
}
