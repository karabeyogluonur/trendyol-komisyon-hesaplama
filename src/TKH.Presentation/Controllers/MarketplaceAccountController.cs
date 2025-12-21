using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TKH.Business.Abstract;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Core.Utilities.Results;
using TKH.Presentation.Models.MarketplaceAccount;
using TKH.Presentation.Services;
using IResult = TKH.Core.Utilities.Results.IResult;

namespace TKH.Presentation.Controllers
{
    public class MarketplaceAccountController : Controller
    {
        private readonly IMarketplaceAccountService _marketplaceService;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public MarketplaceAccountController(
            IMarketplaceAccountService marketplaceService,
            IMapper mapper,
            INotificationService notificationService)
        {
            _marketplaceService = marketplaceService;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IDataResult<List<MarketplaceAccountSummaryDto>> marketplaceAccountListResult = await _marketplaceService.GetAllAsync();

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
        public async Task<IActionResult> Add(MarketplaceAccountAddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _notificationService.Warning("Lütfen zorunlu alanları kontrol ediniz.");
                return View(model);
            }

            MarketplaceAccountAddDto marketplaceAccountAddDto = _mapper.Map<MarketplaceAccountAddDto>(model);

            IResult marketplaceAccountAddResult = await _marketplaceService.AddAsync(marketplaceAccountAddDto);

            if (marketplaceAccountAddResult.Success)
            {
                _notificationService.Success(marketplaceAccountAddResult.Message);
                return RedirectToAction("Index");
            }

            _notificationService.Error(marketplaceAccountAddResult.Message);
            ModelState.AddModelError("", marketplaceAccountAddResult.Message);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            IDataResult<MarketplaceAccountUpdateDto> marketplaceAccountUpdateDtoResult = await _marketplaceService.GetByIdAsync(id);

            if (!marketplaceAccountUpdateDtoResult.Success)
            {
                _notificationService.Error(marketplaceAccountUpdateDtoResult.Message);
                return RedirectToAction("Index");
            }

            var marketplaceAccountUpdateViewModel = _mapper.Map<MarketplaceAccountUpdateViewModel>(marketplaceAccountUpdateDtoResult.Data);
            return View(marketplaceAccountUpdateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MarketplaceAccountUpdateViewModel marketplaceAccountUpdateViewModel)
        {
            if (!ModelState.IsValid)
            {
                _notificationService.Warning("Lütfen girdiğiniz bilgileri kontrol ediniz.");
                return View(marketplaceAccountUpdateViewModel);
            }

            MarketplaceAccountUpdateDto marketplaceAccountUpdateDto = _mapper.Map<MarketplaceAccountUpdateDto>(marketplaceAccountUpdateViewModel);
            IResult marketplaceAccountUpdateResult = await _marketplaceService.UpdateAsync(marketplaceAccountUpdateDto);

            if (marketplaceAccountUpdateResult.Success)
            {
                _notificationService.Success(marketplaceAccountUpdateResult.Message);
                return RedirectToAction("Index");
            }

            _notificationService.Error(marketplaceAccountUpdateResult.Message);
            ModelState.AddModelError("", marketplaceAccountUpdateResult.Message);
            return View(marketplaceAccountUpdateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            IResult marketplaceAccountDeleteResult = await _marketplaceService.DeleteAsync(id);

            if (marketplaceAccountDeleteResult.Success)
                _notificationService.Success(marketplaceAccountDeleteResult.Message);
            else
                _notificationService.Error(marketplaceAccountDeleteResult.Message);

            return RedirectToAction("Index");
        }
    }
}
