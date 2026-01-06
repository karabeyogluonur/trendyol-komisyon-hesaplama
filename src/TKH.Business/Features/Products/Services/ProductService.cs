using AutoMapper;
using AutoMapper.QueryableExtensions;
using TKH.Business.Features.Categories.Dtos;
using TKH.Business.Features.Products.Dtos;
using TKH.Core.DataAccess;
using TKH.Core.Utilities.Paging;
using TKH.Core.Utilities.Results;
using TKH.DataAccess.Extensions;
using TKH.Entities;
using Microsoft.EntityFrameworkCore;
using TKH.Business.Features.Products.Enums;
using TKH.Entities.Enums;


namespace TKH.Business.Features.Products.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _productRepository = _unitOfWork.GetRepository<Product>();
            _mapper = mapper;
        }
        public async Task<IDataResult<IPagedList<ProductSummaryDto>>> GetPagedListAsync(ProductListFilterDto productListFilterDto)
        {
            IQueryable<Product> query = _productRepository.Query();

            if (!string.IsNullOrEmpty(productListFilterDto.Barcode))
                query = query.Where(product => product.Barcode == productListFilterDto.Barcode);

            if (productListFilterDto.CategoryId.HasValue || productListFilterDto.CategoryId > 0)
                query = query.Where(product => product.CategoryId == productListFilterDto.CategoryId);

            if (productListFilterDto.HasStock.HasValue)
                query = query.Where(product => product.StockQuantity > 0);

            if (productListFilterDto.IsOnSale.HasValue)
                query = query.Where(product => product.IsOnSale == productListFilterDto.IsOnSale);

            IQueryable<ProductSummaryDto> productSummaryDtos = query.ProjectTo<ProductSummaryDto>(_mapper.ConfigurationProvider);

            IPagedList<ProductSummaryDto> pagedProductSummaryDto = await productSummaryDtos.ToPagedListAsync(productListFilterDto.PageIndex, productListFilterDto.PageSize);

            return new SuccessDataResult<IPagedList<ProductSummaryDto>>(pagedProductSummaryDto);
        }

        public async Task<IDataResult<List<CategoryLookupDto>>> GetUsedCategoriesAsync()
        {
            IQueryable<Product> query = _productRepository.Query();

            IQueryable<CategoryLookupDto> categoryLookupDtos = query
                .Where(product => product.CategoryId != null)
                .Select(product => product.Category)
                .Distinct()
                .ProjectTo<CategoryLookupDto>(_mapper.ConfigurationProvider);

            List<CategoryLookupDto> categories = await categoryLookupDtos.ToListAsync();

            return new SuccessDataResult<List<CategoryLookupDto>>(categories);
        }

        public async Task<IDataResult<IPagedList<ProductProfitSummaryDto>>> GetPagedProductProfitListAsync(ProductProfitListFilterDto productProfitListFilterDto)
        {
            IQueryable<Product> query = _productRepository.Query();

            if (!string.IsNullOrEmpty(productProfitListFilterDto.Barcode))
                query = query.Where(product => product.Barcode == productProfitListFilterDto.Barcode);

            if (productProfitListFilterDto.CategoryId.HasValue || productProfitListFilterDto.CategoryId > 0)
                query = query.Where(product => product.CategoryId == productProfitListFilterDto.CategoryId);

            if (productProfitListFilterDto.HasStock.HasValue)
                query = query.Where(product => product.StockQuantity > 0);

            if (productProfitListFilterDto.IsOnSale.HasValue)
                query = query.Where(product => product.IsOnSale == productProfitListFilterDto.IsOnSale);

            IQueryable<ProductProfitSummaryDto> productProfitSummaryDtos = query.ProjectTo<ProductProfitSummaryDto>(_mapper.ConfigurationProvider);

            IPagedList<ProductProfitSummaryDto> pagedProductProfitSummaryDto = await productProfitSummaryDtos.OrderBy(product => product.ModelCode).ToPagedListAsync(productProfitListFilterDto.PageIndex, productProfitListFilterDto.PageSize);

            return new SuccessDataResult<IPagedList<ProductProfitSummaryDto>>(pagedProductProfitSummaryDto);
        }

        public async Task<IDataResult<IPagedList<ProductCostSummaryDto>>> GetPagedProductCostListAsync(ProductCostListFilterDto productCostListFilterDto)
        {
            IQueryable<Product> query = _productRepository.Query().Include(product => product.Expenses).Include(product => product.Prices);

            if (!string.IsNullOrEmpty(productCostListFilterDto.Barcode))
                query = query.Where(product => product.Barcode == productCostListFilterDto.Barcode);

            if (productCostListFilterDto.CategoryId.HasValue || productCostListFilterDto.CategoryId > 0)
                query = query.Where(product => product.CategoryId == productCostListFilterDto.CategoryId);

            if (productCostListFilterDto.HasStock.HasValue)
                query = query.Where(product => product.StockQuantity > 0);

            if (productCostListFilterDto.IsOnSale.HasValue)
                query = query.Where(product => product.IsOnSale == productCostListFilterDto.IsOnSale);

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
                        product.Expenses.Any(expense => expense.Type == ProductExpenseType.CommissionRate && expense.EndDate == null && expense.Amount > 0)
                    );
                    break;

                case ProductCostFilterType.All:
                default:
                    break;
            }

            IQueryable<ProductCostSummaryDto> productCostSummaryDtos = query.ProjectTo<ProductCostSummaryDto>(_mapper.ConfigurationProvider);

            IPagedList<ProductCostSummaryDto> pagedProductProfitSummaryDto = await productCostSummaryDtos.OrderBy(product => product.ModelCode).ToPagedListAsync(productCostListFilterDto.PageIndex, productCostListFilterDto.PageSize);

            return new SuccessDataResult<IPagedList<ProductCostSummaryDto>>(pagedProductProfitSummaryDto);


        }

        public async Task<IDataResult<ProductSummaryDto>> GetByIdAsync(int productId)
        {
            var product = await _productRepository.Query().Where(product => product.Id == productId).ProjectTo<ProductSummaryDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

            if (product is null)
                return new ErrorDataResult<ProductSummaryDto>("Ürün bulunamadı.");

            return new SuccessDataResult<ProductSummaryDto>(product);

        }

        public async Task<IDataResult<List<ProductSummaryDto>>> GetByIdsAsync(IEnumerable<int> productIds)
        {
            if (productIds is null || !productIds.Any())
                return new SuccessDataResult<List<ProductSummaryDto>>(new List<ProductSummaryDto>());

            List<ProductSummaryDto> products = await _productRepository
                .Query()
                .Where(product => productIds.Contains(product.Id))
                .ProjectTo<ProductSummaryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new SuccessDataResult<List<ProductSummaryDto>>(products);
        }
    }
}
