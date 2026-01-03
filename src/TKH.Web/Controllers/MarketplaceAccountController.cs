using Microsoft.AspNetCore.Mvc;
using TKH.Core.Utilities.Results;
using TKH.Web.Features.MarketplaceAccounts.Models;
using TKH.Web.Features.MarketplaceAccounts.Services;
using IResult = TKH.Core.Utilities.Results.IResult;

namespace TKH.Web.Controllers
{
    public class MarketplaceAccountController : BaseController
    {
        private readonly IMarketplaceAccountOrchestrator _marketplaceAccountOrchestrator;

        public MarketplaceAccountController(IMarketplaceAccountOrchestrator marketplaceAccountOrchestrator)
        {
            _marketplaceAccountOrchestrator = marketplaceAccountOrchestrator;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IDataResult<List<MarketplaceAccountListViewModel>> prepareMarketplaceAccountListViewModelResult = await _marketplaceAccountOrchestrator.PrepareMarketplaceAccountListViewModelAsync();

            if (!prepareMarketplaceAccountListViewModelResult.Success)
                return View(new List<MarketplaceAccountListViewModel>());

            return View(prepareMarketplaceAccountListViewModelResult.Data);
        }

        [HttpGet]
        public IActionResult Add()
        {
            MarketplaceAccountAddViewModel marketplaceAccountAddViewModel = _marketplaceAccountOrchestrator.PrepareMarketplaceAccountAddViewModel();

            return View(marketplaceAccountAddViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(MarketplaceAccountAddViewModel marketplaceAccountAddViewModel)
        {
            if (!ModelState.IsValid)
                return View(marketplaceAccountAddViewModel);

            IResult createMarketplaceAccountResult = await _marketplaceAccountOrchestrator.CreateMarketplaceAccountAsync(marketplaceAccountAddViewModel);

            return await HandleResultAsync(createMarketplaceAccountResult,
                () => Task.FromResult<IActionResult>(
                    RedirectToAction(nameof(Index))),
                () => Task.FromResult<IActionResult>(
                    View(marketplaceAccountAddViewModel)));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            IDataResult<MarketplaceAccountUpdateViewModel> prepareMarketplaceAccountUpdateViewModelResult =
                await _marketplaceAccountOrchestrator.PrepareMarketplaceAccountUpdateViewModelAsync(id);

            if (!prepareMarketplaceAccountUpdateViewModelResult.Success)
                return RedirectToAction(nameof(Index));

            return View(prepareMarketplaceAccountUpdateViewModelResult.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MarketplaceAccountUpdateViewModel marketplaceAccountUpdateViewModel)
        {
            if (!ModelState.IsValid)
            {
                await _marketplaceAccountOrchestrator.PrepareMarketplaceAccountUpdateViewModelAsync(marketplaceAccountUpdateViewModel.Id);

                return View(marketplaceAccountUpdateViewModel);
            }

            IResult updateMarketplaceAccountResult =
                await _marketplaceAccountOrchestrator.UpdateMarketplaceAccountAsync(
                    marketplaceAccountUpdateViewModel);

            return await HandleResultAsync(updateMarketplaceAccountResult,
                () => Task.FromResult<IActionResult>(
                    RedirectToAction(nameof(Index))),
                async () =>
                {
                    await _marketplaceAccountOrchestrator.PrepareMarketplaceAccountUpdateViewModelAsync(marketplaceAccountUpdateViewModel.Id);

                    return View(marketplaceAccountUpdateViewModel);
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            IResult deleteMarketplaceAccountResult = await _marketplaceAccountOrchestrator.DeleteMarketplaceAccountAsync(id);

            return await HandleResultAsync(deleteMarketplaceAccountResult,
                () => Task.FromResult<IActionResult>(
                    RedirectToAction(nameof(Index))),
                () => Task.FromResult<IActionResult>(
                    RedirectToAction(nameof(Index))));
        }
    }
}
