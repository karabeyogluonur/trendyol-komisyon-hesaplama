using Microsoft.AspNetCore.Mvc;
using TKH.Core.Utilities.Results;
using TKH.Presentation.Features.Products.Models;
using TKH.Presentation.Features.Products.Services;
using TKH.Presentation.Infrastructure.Filters;

namespace TKH.Presentation.Controllers
{
    [EnsureMarketplaceAccountSelected]
    public class ProductController : BaseController
    {
        private readonly IProductOrchestrator _productOrchestrator;

        public ProductController(IProductOrchestrator productOrchestrator)
        {
            _productOrchestrator = productOrchestrator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ProductListFilterViewModel productListFilterViewModel)
        {
            IDataResult<ProductListViewModel> prepareProductListViewModelResult = await _productOrchestrator.PrepareProductListViewModelAsync(productListFilterViewModel);

            if (!prepareProductListViewModelResult.Success)
                return View(new ProductListViewModel { Filter = productListFilterViewModel });

            return View(prepareProductListViewModelResult.Data);
        }
    }
}
