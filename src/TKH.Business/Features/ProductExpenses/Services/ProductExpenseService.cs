using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using TKH.Business.Common.Services;
using TKH.Business.Features.ProductExpenses.Dtos;
using TKH.Business.Features.Products.Dtos;
using TKH.Business.Features.Products.Services;
using TKH.Core.Common.Settings;
using TKH.Core.DataAccess;
using TKH.Core.Utilities.Results;
using TKH.Entities;
using TKH.Entities.Enums;

namespace TKH.Business.Features.ProductExpenses.Services
{
    public class ProductExpenseService : IProductExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Product> _productRepository;
        private readonly IProductService _productService;
        private readonly IMarketplaceTaxService _marketplaceTaxService;
        private readonly TaxSettings _taxSettings;
        private readonly ILogger<ProductExpenseService> _logger;

        public ProductExpenseService(
            IUnitOfWork unitOfWork,
            IProductService productService,
            IMarketplaceTaxService marketplaceTaxService,
            TaxSettings taxSettings,
            ILogger<ProductExpenseService> logger)
        {
            _unitOfWork = unitOfWork;
            _productRepository = _unitOfWork.GetRepository<Product>();
            _productService = productService;
            _marketplaceTaxService = marketplaceTaxService;
            _taxSettings = taxSettings;
            _logger = logger;
        }

        public async Task<IResult> CreateProductExpenseAsync(ProductExpenseCreateDto productExpenseCreateDto)
        {
            _logger.LogInformation("Creating product expense started for ProductId: {ProductId}, Type: {Type}", productExpenseCreateDto.ProductId, productExpenseCreateDto.Type);

            Product? productEntity = await _productRepository.GetFirstOrDefaultAsync(
                predicate: product => product.Id == productExpenseCreateDto.ProductId,
                include: source => source.Include(product => product.Expenses),
                disableTracking: false
            );

            if (productEntity is null)
            {
                _logger.LogWarning("CreateProductExpenseAsync failed. Product not found. ProductId: {ProductId}", productExpenseCreateDto.ProductId);
                return new ErrorResult("Ürün bulunamadı.");
            }

            IDataResult<ProductSummaryDto> getProductResult = await _productService.GetProductByIdAsync(productExpenseCreateDto.ProductId);

            decimal calculatedVatRate = ResolveVatRate(productExpenseCreateDto.Type, getProductResult.Data.MarketplaceType);

            productEntity.AddOrUpdateExpense(
                productExpenseCreateDto.Type,
                productExpenseCreateDto.GenerationType,
                productExpenseCreateDto.Amount,
                calculatedVatRate,
                isVatIncluded: true
            );

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product expense processed successfully for ProductId: {ProductId}", productExpenseCreateDto.ProductId);

            return new SuccessResult("Ürün gideri başarıyla güncellendi.");
        }

        public async Task<IResult> CreateProductExpensesAsync(List<ProductExpenseCreateDto> productExpenseCreateDtos)
        {
            if (productExpenseCreateDtos is null || !productExpenseCreateDtos.Any())
            {
                _logger.LogWarning("CreateProductExpensesAsync called with empty list.");
                return new SuccessResult("İşlenecek kayıt bulunamadı.");
            }

            _logger.LogInformation("Bulk creating product expenses. Count: {Count}", productExpenseCreateDtos.Count);

            List<int> distinctProductIds = productExpenseCreateDtos.Select(productExpenseCreateDtos => productExpenseCreateDtos.ProductId).Distinct().ToList();

            IList<Product> productEntities = await _productRepository.GetAllAsync(
                predicate: product => distinctProductIds.Contains(product.Id),
                include: source => source.Include(product => product.Expenses),
                disableTracking: false
            );

            if (!productEntities.Any())
            {
                _logger.LogWarning("No valid products found matching the input list.");
                return new ErrorResult("İşlenecek geçerli ürün bulunamadı.");
            }

            IDataResult<List<ProductSummaryDto>> getProductsResult = await _productService.GetProductsByIdsAsync(distinctProductIds);
            var productMarketplaceMap = getProductsResult.Data.ToDictionary(product => product.Id, product => product.MarketplaceType);

            Dictionary<(MarketplaceType, ProductExpenseType), decimal> vatRateCacheDictionary = new Dictionary<(MarketplaceType, ProductExpenseType), decimal>();

            int processedCount = 0;

            foreach (ProductExpenseCreateDto productExpenseCreateDto in productExpenseCreateDtos)
            {
                Product? productEntity = productEntities.FirstOrDefault(product => product.Id == productExpenseCreateDto.ProductId);

                if (productEntity is null)
                    continue;

                if (!productMarketplaceMap.TryGetValue(productEntity.Id, out MarketplaceType marketplaceType))
                    continue;

                (MarketplaceType, ProductExpenseType) vatRateCacheKey = (marketplaceType, productExpenseCreateDto.Type);

                if (!vatRateCacheDictionary.TryGetValue(vatRateCacheKey, out decimal vatRate))
                {
                    vatRate = ResolveVatRate(productExpenseCreateDto.Type, marketplaceType);
                    vatRateCacheDictionary[vatRateCacheKey] = vatRate;
                }

                productEntity.AddOrUpdateExpense(
                    productExpenseCreateDto.Type,
                    productExpenseCreateDto.GenerationType,
                    productExpenseCreateDto.Amount,
                    vatRate,
                    isVatIncluded: true
                );

                processedCount++;
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Bulk expense creation completed. Processed count: {ProcessedCount}", processedCount);
            return new SuccessResult($"{processedCount} adet gider kaydı işlendi.");
        }

        private decimal ResolveVatRate(ProductExpenseType productExpenseType, MarketplaceType marketplaceType)
        {
            switch (productExpenseType)
            {
                case ProductExpenseType.ShippingCost:
                    return _taxSettings.ShippingVatRate;

                default:
                    return _marketplaceTaxService.GetVatRateByExpenseType(marketplaceType, productExpenseType);
            }
        }
    }
}
