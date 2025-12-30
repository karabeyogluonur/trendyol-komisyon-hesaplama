using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TKH.Core.Contexts;
using TKH.Presentation.Services;

namespace TKH.Presentation.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EnsureMarketplaceAccountSelectedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            IWorkContext? workContext = context.HttpContext.RequestServices.GetService<IWorkContext>();
            INotificationService? notificationService = context.HttpContext.RequestServices.GetService<INotificationService>();

            if (workContext is null || !workContext.CurrentMarketplaceAccountId.HasValue)
            {
                if (notificationService is not null)
                    notificationService.Warning("Seçili mağaza bulunamadı. Lütfen mağaza seçiniz!");

                context.Result = new RedirectToActionResult("Index", "Home", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
