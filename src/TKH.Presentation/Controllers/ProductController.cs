using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TKH.Business.Abstract;
using TKH.Business.Dtos.Product;
using TKH.Core.Utilities.Paging;
using TKH.Core.Utilities.Results;
using TKH.Presentation.Models.Product;

namespace TKH.Presentation.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ProductListFilterViewModel productListFilterViewModel)
        {
            ProductListFilterDto productListFilterDto = _mapper.Map<ProductListFilterDto>(productListFilterViewModel);

            IDataResult<IPagedList<ProductSummaryDto>> productPagedListResult = await _productService.GetPagedListAsync(productListFilterDto);

            ProductListViewModel productListViewModel = new ProductListViewModel
            {
                Products = _mapper.Map<IPagedList<ProductListItemViewModel>>(productPagedListResult.Data),
                Filter = productListFilterViewModel,
            };

            return View(productListViewModel);
        }
    }
}
