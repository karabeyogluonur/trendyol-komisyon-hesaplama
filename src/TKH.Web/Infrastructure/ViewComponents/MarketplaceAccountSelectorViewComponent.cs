using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Business.Features.MarketplaceAccounts.Services;
using TKH.Core.Contexts;
using TKH.Core.Utilities.Results;
using TKH.Web.Features.MarketplaceAccounts.Models;

namespace TKH.Web.Infrastructure.ViewComponents
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
            IDataResult<List<MarketplaceAccountSummaryDto>> activeMarketplaceAccountResult = await _marketplaceService.GetActiveMarketplaceAccountsAsync();

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
