using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using TKH.Business.Features.Categories.Dtos;
using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Business.Features.MarketplaceAccounts.Services;
using TKH.Business.Features.Products.Dtos;
using TKH.Business.Features.Products.Services;
using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Factories;
using TKH.Core.Common.Settings;
using TKH.Core.Contexts;
using TKH.Core.Utilities.Paging;
using TKH.Core.Utilities.Results;
using TKH.Web.Features.Common.Models;
using TKH.Web.Features.ProductProfits.Models;
using TKH.Web.Features.Products.Models;

namespace TKH.Web.Features.ProductProfits.Services
{
    public class ProductProfitOrchestrator : IProductProfitOrchestrator
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IWorkContext _workContext;
        private readonly IMarketplaceAccountService _marketplaceAccountService;
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;
        private readonly TaxSettings _taxSettings;

        public ProductProfitOrchestrator(IProductService productService, IMapper mapper, IWorkContext workContext, IMarketplaceAccountService marketplaceAccountService, MarketplaceProviderFactory marketplaceProviderFactory, TaxSettings taxSettings)
        {
            _productService = productService;
            _workContext = workContext;
            _mapper = mapper;
            _marketplaceAccountService = marketplaceAccountService;
            _marketplaceProviderFactory = marketplaceProviderFactory;
            _taxSettings = taxSettings;
        }

        public async Task<IDataResult<ProductProfitListViewModel>> PrepareProductProfitListViewModelAsync(ProductProfitListFilterViewModel productProfitListFilterViewModel)
        {
            IDataResult<MarketplaceAccountDetailsDto> getMarketplaceAccountResult = await _marketplaceAccountService.GetMarketplaceAccountByIdAsync(_workContext.CurrentMarketplaceAccountId.Value);

            if (!getMarketplaceAccountResult.Success)
                return new ErrorDataResult<ProductProfitListViewModel>(getMarketplaceAccountResult.Message);

            ProductProfitListFilterDto productProfitListFilterDto = _mapper.Map<ProductProfitListFilterDto>(productProfitListFilterViewModel);

            IDataResult<IPagedList<ProductProfitSummaryDto>> productPagedListResult = await _productService.GetPagedProductProfitListAsync(productProfitListFilterDto);

            if (!productPagedListResult.Success)
                return new ErrorDataResult<ProductProfitListViewModel>(productPagedListResult.Message);

            IDataResult<List<CategoryLookupDto>> usedCategoriesResult = await _productService.GetUsedCategoriesAsync();

            if (usedCategoriesResult.Success)
            {
                productProfitListFilterViewModel.Categories = usedCategoriesResult.Data.Select(category => new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name,
                    Selected = productProfitListFilterViewModel.CategoryId.HasValue && productProfitListFilterViewModel.CategoryId.Value == category.Id
                }).ToList();
            }

            IMarketplaceDefaultsProvider marketplaceDefaultsProvider = _marketplaceProviderFactory.GetProvider<IMarketplaceDefaultsProvider>(getMarketplaceAccountResult.Data.MarketplaceType);


            ProductProfitListViewModel productProfitListViewModel = new ProductProfitListViewModel
            {
                Products = _mapper.Map<IPagedList<ProductProfitListItemViewModel>>(productPagedListResult.Data),
                Filter = productProfitListFilterViewModel,
                MarketplaceType = getMarketplaceAccountResult.Data.MarketplaceType,
                MarketplaceDefaults = _mapper.Map<MarketplaceDefaultsViewModel>(marketplaceDefaultsProvider.GetDefaults()),
                WithholdingRate = _taxSettings.WithholdingRate,
                ShippingVatRate = _taxSettings.ShippingVatRate
            };

            return new SuccessDataResult<ProductProfitListViewModel>(productProfitListViewModel);
        }
    }
}
