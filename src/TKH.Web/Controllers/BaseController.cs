using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TKH.Core.Utilities.Results;
using TKH.Web.Infrastructure.Services;
using IResult = TKH.Core.Utilities.Results.IResult;

namespace TKH.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected INotificationService NotificationService =>
            HttpContext.RequestServices.GetRequiredService<INotificationService>();

        protected async Task<IActionResult> HandleResultAsync(IResult operationResult, Func<Task<IActionResult>> successAction, Func<Task<IActionResult>> failureAction)
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

        protected IActionResult HandleJsonResult(IResult result)
        {
            if (result is null)
                return BadRequest(new { success = false, message = "Sunucudan geçerli bir yanıt alınamadı." });

            var response = new
            {
                success = result.Success,
                message = result.Message
            };

            if (result.Success)
                return Ok(response);

            return Ok(response);
        }

        protected IActionResult HandleJsonResult<T>(IDataResult<T> result)
        {
            if (result is null)
                return Ok(new { success = false, message = "Sunucudan geçerli bir veri alınamadı." });

            var response = new
            {
                success = result.Success,
                message = result.Message,
                data = result.Data
            };

            return Ok(response);
        }

        protected IActionResult HandleValidationJsonResult(ModelStateDictionary modelState)
        {
            var errors = modelState.Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return BadRequest(new
            {
                success = false,
                message = "Lütfen hatalı alanları kontrol ediniz.",
                validationErrors = errors
            });
        }
    }
}

