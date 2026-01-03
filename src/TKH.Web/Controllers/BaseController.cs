using Microsoft.AspNetCore.Mvc;
using TKH.Web.Infrastructure.Services;
using IResult = TKH.Core.Utilities.Results.IResult;

namespace TKH.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected INotificationService NotificationService =>
            HttpContext.RequestServices.GetRequiredService<INotificationService>();

        protected async Task<IActionResult> HandleResultAsync(
            IResult operationResult,
            Func<Task<IActionResult>> successAction,
            Func<Task<IActionResult>> failureAction)
        {
            if (operationResult is null)
            {
                NotificationService.Error("Beklenmeyen bir hata oluştu.");
                return await failureAction();
            }

            if (operationResult.Success)
            {
                if (!string.IsNullOrWhiteSpace(operationResult.Message))
                    NotificationService.Success(operationResult.Message);

                return await successAction();
            }

            if (!string.IsNullOrWhiteSpace(operationResult.Message))
                NotificationService.Error(operationResult.Message);

            return await failureAction();
        }
    }
}
