using Microsoft.AspNetCore.Mvc;
using TKH.Business.Abstract;
using TKH.Presentation.Services;

namespace TKH.Presentation.Controllers
{
    public class CommonController : Controller
    {
        private readonly IWorkContext _workContext;
        private readonly INotificationService _notificationService;

        public CommonController(IWorkContext workContext, INotificationService notificationService)
        {
            _workContext = workContext;
            _notificationService = notificationService;
        }

        [HttpPost]
        public IActionResult SetCurrentMarketplaceAccount(int accountId, string returnUrl)
        {
            _workContext.CurrentMarketplaceAccountId = accountId;

            _notificationService.Info("Aktif pazaryeri değiştirildi.");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
