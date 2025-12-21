using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TKH.Business.Abstract;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Core.Utilities.Results;
using TKH.Presentation.Models.MarketplaceAccount;

namespace TKH.Presentation.ViewComponents
{
    public class MarketplaceAccountSelectorViewComponent : ViewComponent
    {
        private readonly IMarketplaceAccountService _marketplaceService;
        private readonly IWorkContext _workContext;
        private readonly IMapper _mapper;

        public MarketplaceAccountSelectorViewComponent(
            IMarketplaceAccountService marketplaceService,
            IMapper mapper,
            IWorkContext workContext)
        {
            _marketplaceService = marketplaceService;
            _workContext = workContext;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IDataResult<List<MarketplaceAccountSummaryDto>> activeMarketplaceAccountResult = await _marketplaceService.GetActiveAccountsAsync();

            List<MarketplaceAccountSummaryDto> marketplaceAccountSummaryDtos = activeMarketplaceAccountResult.Success ? activeMarketplaceAccountResult.Data : new List<MarketplaceAccountSummaryDto>();

            List<MarketplaceAccountSelectorItemViewModel> marketplaceAccountSelectorItemViewModels = _mapper.Map<List<MarketplaceAccountSelectorItemViewModel>>(marketplaceAccountSummaryDtos);

            int? currentMarketplaceAccountId = _workContext.CurrentMarketplaceAccountId;

            MarketplaceAccountSelectorItemViewModel currentMarketplaceAccountSelectorItemViewModel = marketplaceAccountSelectorItemViewModels.FirstOrDefault(x => x.Id == currentMarketplaceAccountId);

            MarketplaceAccountSelectorViewModel marketplaceAccountSelectorViewModel = new MarketplaceAccountSelectorViewModel
            {
                MarketplaceAccounts = marketplaceAccountSelectorItemViewModels,
                CurrentMarketplaceAccountId = currentMarketplaceAccountId,
                CurrentMarketplaceAccount = currentMarketplaceAccountSelectorItemViewModel
            };

            return View(marketplaceAccountSelectorViewModel);
        }
    }
}
