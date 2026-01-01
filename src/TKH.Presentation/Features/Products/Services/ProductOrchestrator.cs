using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using TKH.Business.Features.Categories.Dtos;
using TKH.Business.Features.Products.Dtos;
using TKH.Business.Features.Products.Services;
using TKH.Core.Utilities.Paging;
using TKH.Core.Utilities.Results;
using TKH.Presentation.Features.Products.Models;

namespace TKH.Presentation.Features.Products.Services
{
    public class ProductOrchestrator : IProductOrchestrator
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductOrchestrator(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        public async Task<IDataResult<ProductListViewModel>> PrepareProductListViewModelAsync(ProductListFilterViewModel productListFilterViewModel)
        {
            ProductListFilterDto productListFilterDto = _mapper.Map<ProductListFilterDto>(productListFilterViewModel);

            IDataResult<IPagedList<ProductSummaryDto>> productPagedListResult = await _productService.GetPagedListAsync(productListFilterDto);

            if (!productPagedListResult.Success)
                return new ErrorDataResult<ProductListViewModel>(productPagedListResult.Message);

            IDataResult<List<CategoryLookupDto>> usedCategoriesResult = await _productService.GetUsedCategoriesAsync();

            if (!usedCategoriesResult.Success)
                return new ErrorDataResult<ProductListViewModel>(usedCategoriesResult.Message);

            productListFilterViewModel.Categories = usedCategoriesResult.Data.Select(category => new SelectListItem
            {
                Value = category.Id.ToString(),
                Text = category.Name,
                Selected = productListFilterViewModel.CategoryId.HasValue && productListFilterViewModel.CategoryId.Value == category.Id
            }).ToList();

            ProductListViewModel productListViewModel = new ProductListViewModel
            {
                Products = _mapper.Map<IPagedList<ProductListItemViewModel>>(productPagedListResult.Data),
                Filter = productListFilterViewModel
            };

            return new SuccessDataResult<ProductListViewModel>(productListViewModel);
        }
    }
}
