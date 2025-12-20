using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TKH.Business.Abstract;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Presentation.Models.MarketplaceAccount;
using TKH.Core.Utilities.Results;
using IResult = TKH.Core.Utilities.Results.IResult;

namespace TKH.Presentation.Controllers
{
    public class MarketplaceAccountController : Controller
    {
        private readonly IMarketplaceAccountService _marketplaceService;
        private readonly IMapper _mapper;

        public MarketplaceAccountController(IMarketplaceAccountService marketplaceService, IMapper mapper)
        {
            _marketplaceService = marketplaceService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IDataResult<List<MarketplaceAccountListDto>> marketplaceAccountListResult = await _marketplaceService.GetAllAsync();

            if (marketplaceAccountListResult.Success)
            {
                var viewModel = _mapper.Map<List<MarketplaceAccountListViewModel>>(marketplaceAccountListResult.Data);
                return View(viewModel);
            }
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
                return View(model);

            MarketplaceAccountAddDto marketplaceAccountAddDto = _mapper.Map<MarketplaceAccountAddDto>(model);

            IResult marketplaceAccountAddResult = await _marketplaceService.AddAsync(marketplaceAccountAddDto);

            if (marketplaceAccountAddResult.Success)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", marketplaceAccountAddResult.Message);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            IDataResult<MarketplaceAccountUpdateDto> marketplaceAccountUpdateDtoResult = await _marketplaceService.GetByIdAsync(id);

            if (!marketplaceAccountUpdateDtoResult.Success)
                return RedirectToAction("Index");

            var model = _mapper.Map<MarketplaceAccountUpdateViewModel>(marketplaceAccountUpdateDtoResult.Data);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MarketplaceAccountUpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            MarketplaceAccountUpdateDto marketplaceAccountUpdateDto = _mapper.Map<MarketplaceAccountUpdateDto>(model);
            IResult marketplaceAccountUpdateResult = await _marketplaceService.UpdateAsync(marketplaceAccountUpdateDto);

            if (marketplaceAccountUpdateResult.Success)
                return RedirectToAction("Index");

            ModelState.AddModelError("", marketplaceAccountUpdateResult.Message);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _marketplaceService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
