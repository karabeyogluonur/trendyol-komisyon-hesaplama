using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using TKH.Business.Features.Categories.Dtos;
using TKH.Business.Features.ProductExpenses.Dtos;
using TKH.Business.Features.ProductExpenses.Services;
using TKH.Business.Features.ProductPrices.Models;
using TKH.Business.Features.ProductPrices.Services;
using TKH.Business.Features.Products.Dtos;
using TKH.Business.Features.Products.Enums;
using TKH.Business.Features.Products.Services;
using TKH.Core.Utilities.Paging;
using TKH.Core.Utilities.Results;
using TKH.Entities.Enums;
using TKH.Web.Features.Products.Models;
using TKH.Web.Configuration.Extensions;
using IResult = TKH.Core.Utilities.Results.IResult;

namespace TKH.Web.Features.Products.Services
{
    public class ProductOrchestrator : IProductOrchestrator
    {
        private readonly IProductService _productService;
        private readonly IProductPriceService _productPriceService;
        private readonly IProductExpenseService _productExpenseService;
        private readonly IMapper _mapper;

        public ProductOrchestrator(
            IProductService productService,
            IProductPriceService productPriceService,
            IProductExpenseService productExpenseService,
            IMapper mapper)
        {
            _productService = productService;
            _productPriceService = productPriceService;
            _productExpenseService = productExpenseService;
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

        public async Task<IDataResult<ProductCostListViewModel>> PrepareProductCostListViewModelAsync(ProductCostListFilterViewModel productCostListFilterViewModel)
        {
            ProductCostListFilterDto productCostListFilterDto = _mapper.Map<ProductCostListFilterDto>(productCostListFilterViewModel);

            IDataResult<IPagedList<ProductCostSummaryDto>> productCostPagedListResult = await _productService.GetPagedProductCostListAsync(productCostListFilterDto);

            if (!productCostPagedListResult.Success)
                return new ErrorDataResult<ProductCostListViewModel>(productCostPagedListResult.Message);

            IDataResult<List<CategoryLookupDto>> usedCategoriesResult = await _productService.GetUsedCategoriesAsync();

            if (usedCategoriesResult.Success)
                productCostListFilterViewModel.Categories = usedCategoriesResult.Data.ToSelectList(category => category.Id.ToString(), category => category.Name, productCostListFilterViewModel.CategoryId?.ToString());

            ProductCostListViewModel productCostListViewModel = new ProductCostListViewModel
            {
                Products = _mapper.Map<IPagedList<ProductCostListItemViewModel>>(productCostPagedListResult.Data),
                Filter = productCostListFilterViewModel
            };

            return new SuccessDataResult<ProductCostListViewModel>(productCostListViewModel);
        }

        public async Task<IResult> UpdateProductCostsAsync(List<ProductCostBatchViewModel> productCostBatchViewModels)
        {
            if (productCostBatchViewModels is null || !productCostBatchViewModels.Any())
                return new ErrorResult("Güncellenecek veri bulunamadı.");


            List<ProductPriceAddDto> priceUpdates = productCostBatchViewModels.Select(item => new ProductPriceAddDto
            {
                ProductId = item.Id,
                Type = ProductPriceType.PurchasePrice,
                Amount = item.PurchasePrice
            }).ToList();

            IResult addPriceResult = await _productPriceService.AddRangeAsync(priceUpdates);

            if (!addPriceResult.Success)
                return new ErrorResult($"Fiyat güncelleme hatası: {addPriceResult.Message}");

            List<ProductExpenseAddDto> expenseUpdates = new List<ProductExpenseAddDto>();

            foreach (var productCostBatchViewModel in productCostBatchViewModels)
            {
                expenseUpdates.Add(new ProductExpenseAddDto
                {
                    ProductId = productCostBatchViewModel.Id,
                    Type = ProductExpenseType.CommissionRate,
                    Amount = productCostBatchViewModel.ManualCommissionRate,
                    GenerationType = GenerationType.Manual
                });

                expenseUpdates.Add(new ProductExpenseAddDto
                {
                    ProductId = productCostBatchViewModel.Id,
                    Type = ProductExpenseType.ShippingCost,
                    Amount = productCostBatchViewModel.ManualShippingCost,
                    GenerationType = GenerationType.Manual
                });
            }

            IResult addProductExpenseResult = await _productExpenseService.AddRangeAsync(expenseUpdates);

            if (!addProductExpenseResult.Success)
                return new ErrorResult($"Gider güncelleme hatası: {addProductExpenseResult.Message}");

            return new SuccessResult($"{productCostBatchViewModels.Count} ürün başarıyla güncellendi.");

        }

    }
}
