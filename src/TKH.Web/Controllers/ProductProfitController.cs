using Microsoft.AspNetCore.Mvc;
using TKH.Core.Utilities.Results;
using TKH.Web.Features.ProductProfits.Models;
using TKH.Web.Features.ProductProfits.Services;
using TKH.Web.Features.Products.Models;
using TKH.Web.Infrastructure.Filters;

namespace TKH.Web.Controllers
{
    [EnsureMarketplaceAccountSelected]
    public class ProductProfitController : BaseController
    {
        private readonly IProductProfitOrchestrator _productProfitOrchestrator;

        public ProductProfitController(IProductProfitOrchestrator productProfitOrchestrator)
        {
            _productProfitOrchestrator = productProfitOrchestrator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ProductProfitListFilterViewModel productProfitFilterViewModel)
        {
            IDataResult<ProductProfitListViewModel> prepareProductProfitListViewModelResult = await _productProfitOrchestrator.PrepareProductProfitListViewModelAsync(productProfitFilterViewModel);

            if (!prepareProductProfitListViewModelResult.Success)
                return View(new ProductProfitListViewModel { Filter = productProfitFilterViewModel });

            return View(prepareProductProfitListViewModelResult.Data);
        }
    }

}
