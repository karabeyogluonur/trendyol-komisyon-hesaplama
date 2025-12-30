using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Business.Features.MarketplaceAccounts.Services;
using TKH.Business.Jobs.Services;
using TKH.Core.Utilities.Results;
using TKH.Presentation.Infrastructure.Services;
using TKH.Presentation.Models.MarketplaceAccount;
using IResult = TKH.Core.Utilities.Results.IResult;

namespace TKH.Presentation.Controllers
{
    public class MarketplaceAccountController : Controller
    {
        private readonly IMarketplaceAccountService _marketplaceAccountService;
        private readonly IMarketplaceJobService _marketplaceJobService;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public MarketplaceAccountController(
            IMarketplaceAccountService marketplaceAccountService,
            IMarketplaceJobService marketplaceJobService,
            IMapper mapper,
            INotificationService notificationService)
        {
            _marketplaceAccountService = marketplaceAccountService;
            _marketplaceJobService = marketplaceJobService;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IDataResult<List<MarketplaceAccountSummaryDto>> marketplaceAccountListResult = await _marketplaceAccountService.GetAllAsync();

            if (marketplaceAccountListResult.Success)
            {
                List<MarketplaceAccountListViewModel> marketplaceAccountListViewModels = _mapper.Map<List<MarketplaceAccountListViewModel>>(marketplaceAccountListResult.Data);
                return View(marketplaceAccountListViewModels);
            }

            _notificationService.Error(marketplaceAccountListResult.Message, "Listeleme Hatası.");
            return View(new List<MarketplaceAccountListViewModel>());
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new MarketplaceAccountAddViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(MarketplaceAccountAddViewModel marketplaceAccountAddViewModel)
        {
            if (!ModelState.IsValid)
            {
                _notificationService.Warning("Lütfen zorunlu alanları kontrol ediniz.");
                return View(marketplaceAccountAddViewModel);
            }

            MarketplaceAccountAddDto marketplaceAccountAddDto = _mapper.Map<MarketplaceAccountAddDto>(marketplaceAccountAddViewModel);

            IDataResult<int> marketplaceAccountAddResult = await _marketplaceAccountService.AddAsync(marketplaceAccountAddDto);

            if (marketplaceAccountAddResult.Success)
            {
                _marketplaceJobService.DispatchImmediateSingleAccountDataSync(marketplaceAccountAddResult.Data);

                _notificationService.Success(marketplaceAccountAddResult.Message);
                return RedirectToAction("Index");
            }

            _notificationService.Error(marketplaceAccountAddResult.Message);
            ModelState.AddModelError("", marketplaceAccountAddResult.Message);
            return View(marketplaceAccountAddViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            IDataResult<MarketplaceAccountDetailsDto> result = await _marketplaceAccountService.GetByIdAsync(id);

            if (!result.Success)
            {
                _notificationService.Error(result.Message);
                return RedirectToAction("Index");
            }

            MarketplaceAccountUpdateViewModel marketplaceAccountUpdateViewModel = _mapper.Map<MarketplaceAccountUpdateViewModel>(result.Data);
            return View(marketplaceAccountUpdateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MarketplaceAccountUpdateViewModel marketplaceAccountUpdateViewModel)
        {
            if (!ModelState.IsValid)
            {
                _notificationService.Warning("Lütfen girdiğiniz bilgileri kontrol ediniz.");
                await ReloadStatusFields(marketplaceAccountUpdateViewModel);
                return View(marketplaceAccountUpdateViewModel);
            }

            MarketplaceAccountUpdateDto marketplaceAccountUpdateDto = _mapper.Map<MarketplaceAccountUpdateDto>(marketplaceAccountUpdateViewModel);

            IResult marketplaceAccountUpdateResult = await _marketplaceAccountService.UpdateAsync(marketplaceAccountUpdateDto);

            if (marketplaceAccountUpdateResult.Success)
            {
                _marketplaceJobService.DispatchImmediateSingleAccountDataSync(marketplaceAccountUpdateViewModel.Id);

                _notificationService.Success(marketplaceAccountUpdateResult.Message);
                return RedirectToAction("Index");
            }

            _notificationService.Error(marketplaceAccountUpdateResult.Message);
            ModelState.AddModelError("", marketplaceAccountUpdateResult.Message);

            await ReloadStatusFields(marketplaceAccountUpdateViewModel);

            return View(marketplaceAccountUpdateViewModel);
        }

        private async Task ReloadStatusFields(MarketplaceAccountUpdateViewModel marketplaceAccountUpdateViewModel)
        {
            IDataResult<MarketplaceAccountDetailsDto> marketplaceAccountDetailsResult = await _marketplaceAccountService.GetByIdAsync(marketplaceAccountUpdateViewModel.Id);

            if (marketplaceAccountDetailsResult.Success && marketplaceAccountDetailsResult.Data != null)
            {
                marketplaceAccountUpdateViewModel.ConnectionState = marketplaceAccountDetailsResult.Data.ConnectionState;
                marketplaceAccountUpdateViewModel.SyncState = marketplaceAccountDetailsResult.Data.SyncState;
                marketplaceAccountUpdateViewModel.LastErrorMessage = marketplaceAccountDetailsResult.Data.LastErrorMessage;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            IResult marketplaceAccountDeleteResult = await _marketplaceAccountService.DeleteAsync(id);

            if (marketplaceAccountDeleteResult.Success)
                _notificationService.Success(marketplaceAccountDeleteResult.Message);
            else
                _notificationService.Error(marketplaceAccountDeleteResult.Message);

            return RedirectToAction("Index");
        }
    }
}
