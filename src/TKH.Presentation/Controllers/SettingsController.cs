using Microsoft.AspNetCore.Mvc;
using TKH.Core.Utilities.Results;
using TKH.Presentation.Features.Settings.Models;
using TKH.Presentation.Features.Settings.Services;
using IResult = TKH.Core.Utilities.Results.IResult;
using TKH.Presentation.Infrastructure.Services;

namespace TKH.Presentation.Controllers
{
    public class SettingsController : BaseController
    {
        private readonly ISettingsOrchestrator _settingsOrchestrator;
        private readonly INotificationService _notificationService;
        public SettingsController(
            ISettingsOrchestrator settingsOrchestrator,
            INotificationService notificationService)
        {
            _settingsOrchestrator = settingsOrchestrator;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Tax()
        {
            IDataResult<TaxSettingsViewModel> prepareTaxSettingsResult = await _settingsOrchestrator.PrepareTaxSettingsViewModelAsync();

            if (!prepareTaxSettingsResult.Success)
            {
                _notificationService.Error(prepareTaxSettingsResult.Message);
                return RedirectToAction("Index", "Home");
            }

            return View(prepareTaxSettingsResult.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tax(TaxSettingsViewModel taxSettingsViewModel)
        {
            if (!ModelState.IsValid)
                return View(taxSettingsViewModel);

            IResult updateTaxSettingsResult = await _settingsOrchestrator.UpdateTaxSettingsAsync(taxSettingsViewModel);

            return await HandleResultAsync(updateTaxSettingsResult,
                () => Task.FromResult<IActionResult>(RedirectToAction(nameof(Tax))),
                () => Task.FromResult<IActionResult>(View(taxSettingsViewModel)));
        }

        [HttpGet]
        public async Task<IActionResult> Trendyol()
        {
            IDataResult<TrendyolSettingsViewModel> prepareTrendyolSettingsResult = await _settingsOrchestrator.PrepareTrendyolSettingsViewModelAsync();

            if (!prepareTrendyolSettingsResult.Success)
            {
                _notificationService.Error(prepareTrendyolSettingsResult.Message);
                return RedirectToAction("Index", "Home");
            }

            return View(prepareTrendyolSettingsResult.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Trendyol(TrendyolSettingsViewModel trendyolSettingsViewModel)
        {
            if (!ModelState.IsValid)
                return View(trendyolSettingsViewModel);

            IResult updateTaxSettingsResult = await _settingsOrchestrator.UpdateTrendyolSettingsAsync(trendyolSettingsViewModel);

            return await HandleResultAsync(updateTaxSettingsResult,
                () => Task.FromResult<IActionResult>(RedirectToAction(nameof(Trendyol))),
                () => Task.FromResult<IActionResult>(View(trendyolSettingsViewModel)));
        }
    }
}
