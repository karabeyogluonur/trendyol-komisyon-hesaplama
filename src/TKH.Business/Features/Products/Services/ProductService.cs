using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Logging;
using TKH.Business.Features.Categories.Dtos;
using TKH.Business.Features.Products.Dtos;
using TKH.Business.Features.Products.Enums;
using TKH.Core.DataAccess;
using TKH.Core.Utilities.Paging;
using TKH.Core.Utilities.Results;
using TKH.DataAccess.Extensions;
using TKH.Entities;
using TKH.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace TKH.Business.Features.Products.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _productRepository = _unitOfWork.GetRepository<Product>();
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IDataResult<IPagedList<ProductSummaryDto>>> GetPagedProductListAsync(ProductListFilterDto productListFilterDto)
        {
            if (productListFilterDto is null)
                return new ErrorDataResult<IPagedList<ProductSummaryDto>>("Filtre parametresi boş olamaz.");

            IQueryable<Product> query = _productRepository.Query();

            if (!string.IsNullOrEmpty(productListFilterDto.Barcode))
                query = query.Where(product => product.Barcode == productListFilterDto.Barcode);

            if (productListFilterDto.CategoryId.HasValue && productListFilterDto.CategoryId > 0)
                query = query.Where(product => product.CategoryId == productListFilterDto.CategoryId);

            if (productListFilterDto.HasStock.HasValue)
                query = productListFilterDto.HasStock.Value
                    ? query.Where(product => product.StockQuantity > 0)
                    : query.Where(product => product.StockQuantity < 1);

            if (productListFilterDto.IsOnSale.HasValue)
                query = query.Where(product => product.IsOnSale == productListFilterDto.IsOnSale);

            IQueryable<ProductSummaryDto> productSummaryDtos = query.ProjectTo<ProductSummaryDto>(_mapper.ConfigurationProvider);
            IPagedList<ProductSummaryDto> pagedProducts = await productSummaryDtos.ToPagedListAsync(productListFilterDto.PageIndex, productListFilterDto.PageSize);

            _logger.LogInformation("Paged product list fetched. PageIndex: {PageIndex}, PageSize: {PageSize}", productListFilterDto.PageIndex, productListFilterDto.PageSize);

            return new SuccessDataResult<IPagedList<ProductSummaryDto>>(pagedProducts);
        }

        public async Task<IDataResult<List<CategoryLookupDto>>> GetUsedCategoriesAsync()
        {
            IQueryable<Product> query = _productRepository.Query();

            IQueryable<CategoryLookupDto> categoryLookupDtos = query
                .Where(product => product.CategoryId.HasValue)
                .Select(product => product.Category)
                .Distinct()
                .ProjectTo<CategoryLookupDto>(_mapper.ConfigurationProvider);

            List<CategoryLookupDto> categories = await categoryLookupDtos.ToListAsync();

            _logger.LogInformation("Used categories fetched. Count: {Count}", categories.Count);

            return new SuccessDataResult<List<CategoryLookupDto>>(categories);
        }

        public async Task<IDataResult<IPagedList<ProductProfitSummaryDto>>> GetPagedProductProfitListAsync(ProductProfitListFilterDto productProfitListFilterDto)
        {
            if (productProfitListFilterDto == null)
                return new ErrorDataResult<IPagedList<ProductProfitSummaryDto>>("Filtre parametresi boş olamaz.");

            IQueryable<Product> query = _productRepository.Query();

            if (!string.IsNullOrEmpty(productProfitListFilterDto.Barcode))
                query = query.Where(product => product.Barcode == productProfitListFilterDto.Barcode);

            if (productProfitListFilterDto.CategoryId.HasValue && productProfitListFilterDto.CategoryId > 0)
                query = query.Where(product => product.CategoryId == productProfitListFilterDto.CategoryId);

            if (productProfitListFilterDto.HasStock.HasValue)
                query = productProfitListFilterDto.HasStock.Value ? query.Where(product => product.StockQuantity > 0) : query.Where(product => product.StockQuantity < 1);

            if (productProfitListFilterDto.IsOnSale.HasValue)
                query = query.Where(product => product.IsOnSale == productProfitListFilterDto.IsOnSale);

            IQueryable<ProductProfitSummaryDto> productProfitSummaryDtos = query.ProjectTo<ProductProfitSummaryDto>(_mapper.ConfigurationProvider);
            IPagedList<ProductProfitSummaryDto> pagedproductProfitSummaryDtos = await productProfitSummaryDtos.OrderBy(product => product.ModelCode).ToPagedListAsync(productProfitListFilterDto.PageIndex, productProfitListFilterDto.PageSize);

            _logger.LogInformation("Paged product profit list fetched. PageIndex: {PageIndex}, PageSize: {PageSize}", productProfitListFilterDto.PageIndex, productProfitListFilterDto.PageSize);

            return new SuccessDataResult<IPagedList<ProductProfitSummaryDto>>(pagedproductProfitSummaryDtos);
        }

        public async Task<IDataResult<IPagedList<ProductCostSummaryDto>>> GetPagedProductCostListAsync(ProductCostListFilterDto productCostListFilterDto)
        {
            if (productCostListFilterDto == null)
                return new ErrorDataResult<IPagedList<ProductCostSummaryDto>>("Filtre parametresi boş olamaz.");

            IQueryable<Product> query = _productRepository.Query()
                .Include(product => product.Prices)
                .Include(product => product.Expenses);

            if (!string.IsNullOrEmpty(productCostListFilterDto.Barcode))
                query = query.Where(product => product.Barcode == productCostListFilterDto.Barcode);

            if (productCostListFilterDto.CategoryId.HasValue && productCostListFilterDto.CategoryId > 0)
                query = query.Where(product => product.CategoryId == productCostListFilterDto.CategoryId);

            if (productCostListFilterDto.HasStock.HasValue)
                query = productCostListFilterDto.HasStock.Value ? query.Where(product => product.StockQuantity > 0) : query.Where(product => product.StockQuantity < 1);

            if (productCostListFilterDto.IsOnSale.HasValue)
                query = query.Where(product => product.IsOnSale == productCostListFilterDto.IsOnSale);

            if (productCostListFilterDto.CostStatus.HasValue)
            {
                switch (productCostListFilterDto.CostStatus)
                {
                    case ProductCostFilterType.MissingPurchasePrice:
                        query = query.Where(product => !product.Prices.Any(price => price.Type == ProductPriceType.PurchasePrice && price.EndDate == null && price.Amount > 0));
                        break;
                    case ProductCostFilterType.MissingShippingCost:
                        query = query.Where(product => !product.Expenses.Any(expense => expense.Type == ProductExpenseType.ShippingCost && expense.EndDate == null && expense.Amount > 0));
                        break;
                    case ProductCostFilterType.MissingCommission:
                        query = query.Where(product => !product.Expenses.Any(expense => expense.Type == ProductExpenseType.CommissionRate && expense.EndDate == null && expense.Amount > 0));
                        break;
                    case ProductCostFilterType.Completed:
                        query = query.Where(product =>
                            product.Prices.Any(price => price.Type == ProductPriceType.PurchasePrice && price.EndDate == null && price.Amount > 0) &&
                            product.Expenses.Any(expense => expense.Type == ProductExpenseType.ShippingCost && expense.EndDate == null && expense.Amount > 0) &&
                            product.Expenses.Any(expense => expense.Type == ProductExpenseType.CommissionRate && expense.EndDate == null && expense.Amount > 0));
                        break;
                }
            }

            IQueryable<ProductCostSummaryDto> costDtos = query.ProjectTo<ProductCostSummaryDto>(_mapper.ConfigurationProvider);
            IPagedList<ProductCostSummaryDto> pagedCost = await costDtos.OrderBy(product => product.ModelCode).ToPagedListAsync(productCostListFilterDto.PageIndex, productCostListFilterDto.PageSize);

            _logger.LogInformation("Paged product cost list fetched. PageIndex: {PageIndex}, PageSize: {PageSize}", productCostListFilterDto.PageIndex, productCostListFilterDto.PageSize);

            return new SuccessDataResult<IPagedList<ProductCostSummaryDto>>(pagedCost);
        }

        public async Task<IDataResult<ProductSummaryDto>> GetProductByIdAsync(int productId)
        {
            if (productId <= 0)
                return new ErrorDataResult<ProductSummaryDto>("Geçersiz ürün Id.");

            var product = await _productRepository.Query()
                .Where(product => product.Id == productId)
                .ProjectTo<ProductSummaryDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (product is null)
                return new ErrorDataResult<ProductSummaryDto>("Ürün bulunamadı.");

            _logger.LogInformation("Product fetched by Id: {ProductId}", productId);

            return new SuccessDataResult<ProductSummaryDto>(product);
        }

        public async Task<IDataResult<List<ProductSummaryDto>>> GetProductsByIdsAsync(IEnumerable<int> productIds)
        {
            if (productIds is null || !productIds.Any())
                return new SuccessDataResult<List<ProductSummaryDto>>(new List<ProductSummaryDto>());

            var products = await _productRepository.Query()
                .Where(product => productIds.Contains(product.Id))
                .ProjectTo<ProductSummaryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            _logger.LogInformation("Products fetched by Ids. Count: {Count}", products.Count);

            return new SuccessDataResult<List<ProductSummaryDto>>(products);
        }
    }
}
