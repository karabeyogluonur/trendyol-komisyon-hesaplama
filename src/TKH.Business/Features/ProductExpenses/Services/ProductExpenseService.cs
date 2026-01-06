using TKH.Business.Features.ProductExpenses.Dtos;
using TKH.Business.Features.Products.Dtos;
using TKH.Business.Features.Products.Services;
using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Business.Integrations.Marketplaces.Factories;
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
        private readonly IRepository<ProductExpense> _productExpenseRepository;
        private readonly IProductService _productService;
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;
        private readonly TaxSettings _taxSettings;

        public ProductExpenseService(IUnitOfWork unitOfWork, IProductService productService, MarketplaceProviderFactory marketplaceProviderFactory, TaxSettings taxSettings)
        {
            _unitOfWork = unitOfWork;
            _productExpenseRepository = _unitOfWork.GetRepository<ProductExpense>();
            _productService = productService;
            _marketplaceProviderFactory = marketplaceProviderFactory;
            _taxSettings = taxSettings;

        }
        public async Task<IResult> AddAsync(ProductExpenseAddDto productExpenseAddDto)
        {
            IDataResult<ProductSummaryDto> getProductResult = await _productService.GetByIdAsync(productExpenseAddDto.ProductId);

            if (!getProductResult.Success)
                return new ErrorResult(getProductResult.Message);

            ProductExpense activeExpense = await _productExpenseRepository.GetFirstOrDefaultAsync(predicate: productExpense => productExpense.ProductId == productExpenseAddDto.ProductId &&
                                productExpense.Type == productExpenseAddDto.Type && productExpense.GenerationType == productExpenseAddDto.GenerationType && productExpense.EndDate == null
            );

            if (activeExpense is not null)
            {
                if (activeExpense.Amount == productExpenseAddDto.Amount)
                    return new SuccessResult("Girilen gider tutarı mevcut aktif tutar ile aynı, işlem yapılmadı.");

                activeExpense.EndDate = DateTime.UtcNow;
                _productExpenseRepository.Update(activeExpense);
            }

            ProductExpense newExpense = new ProductExpense
            {
                ProductId = productExpenseAddDto.ProductId,
                Type = productExpenseAddDto.Type,
                Amount = productExpenseAddDto.Amount,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                GenerationType = productExpenseAddDto.GenerationType,
                IsVatIncluded = true,
                VatRate = GetVatRateByMarketplaceType(productExpenseAddDto.Type, getProductResult.Data.MarketplaceType)
            };

            await _productExpenseRepository.InsertAsync(newExpense);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Ürün gideri başarıyla güncellendi.");
        }
        public async Task<IResult> AddRangeAsync(List<ProductExpenseAddDto> productExpenseAddDtos)
        {
            if (productExpenseAddDtos == null || !productExpenseAddDtos.Any())
                return new SuccessResult("İşlenecek kayıt bulunamadı.");

            var productIds = productExpenseAddDtos.Select(x => x.ProductId).Distinct().ToList();

            var getProductsResult = await _productService.GetByIdsAsync(productIds);

            if (!getProductsResult.Success)
                return new ErrorResult(getProductsResult.Message);

            var products = getProductsResult.Data;

            var foundedProductIds = products.Select(product => product.Id).ToList();

            var foundedProductExpenseAddDtos = productExpenseAddDtos.Where(productExpense => foundedProductIds.Contains(productExpense.ProductId)).ToList();

            if (!foundedProductExpenseAddDtos.Any())
                return new ErrorResult("İşlenecek geçerli ürün bulunamadı.");

            IList<ProductExpense> existingExpenses = await _productExpenseRepository.GetAllAsync(predicate: productExpense => productIds.Contains(productExpense.ProductId) && productExpense.EndDate == null, disableTracking: false);

            var vatRateCache = new Dictionary<(MarketplaceType, ProductExpenseType), decimal>();
            bool anyChanges = false;

            foreach (var foundedProductExpenseAddDto in foundedProductExpenseAddDtos)
            {
                ProductExpense? existingExpense = existingExpenses.FirstOrDefault(productExpense =>
                    productExpense.ProductId == foundedProductExpenseAddDto.ProductId &&
                    productExpense.Type == foundedProductExpenseAddDto.Type &&
                    productExpense.GenerationType == foundedProductExpenseAddDto.GenerationType);

                if (existingExpense is not null && existingExpense.Amount == foundedProductExpenseAddDto.Amount)
                    continue;

                if (existingExpense is not null)
                {
                    existingExpense.EndDate = DateTime.UtcNow;
                    _productExpenseRepository.Update(existingExpense);
                }

                ProductSummaryDto product = products.First(product => product.Id == foundedProductExpenseAddDto.ProductId);

                var cacheKey = (product.MarketplaceType, foundedProductExpenseAddDto.Type);
                if (!vatRateCache.TryGetValue(cacheKey, out decimal vatRate))
                {
                    vatRate = GetVatRateByMarketplaceType(foundedProductExpenseAddDto.Type, product.MarketplaceType);
                    vatRateCache[cacheKey] = vatRate;
                }

                var newExpense = new ProductExpense
                {
                    ProductId = foundedProductExpenseAddDto.ProductId,
                    Type = foundedProductExpenseAddDto.Type,
                    Amount = foundedProductExpenseAddDto.Amount,
                    StartDate = DateTime.UtcNow,
                    EndDate = null,
                    GenerationType = foundedProductExpenseAddDto.GenerationType,
                    IsVatIncluded = true,
                    VatRate = vatRate
                };

                await _productExpenseRepository.InsertAsync(newExpense);
                anyChanges = true;
            }

            if (anyChanges)
            {
                await _unitOfWork.SaveChangesAsync();
                return new SuccessResult($"{foundedProductExpenseAddDtos.Count} adet gider kaydı güncellendi/eklendi.");
            }

            return new SuccessResult("Tüm kayıtlar zaten güncel, değişiklik yapılmadı.");
        }
        private decimal GetVatRateByMarketplaceType(ProductExpenseType productExpenseType, MarketplaceType marketplaceType)
        {
            IMarketplaceDefaultsProvider marketplaceDefaultsProvider = _marketplaceProviderFactory.GetProvider<IMarketplaceDefaultsProvider>(marketplaceType);

            if (marketplaceDefaultsProvider is null)
                return 0;

            MarketplaceDefaultsDto marketplaceDefaultsDto = marketplaceDefaultsProvider.GetDefaults();

            switch (productExpenseType)
            {
                case ProductExpenseType.CommissionRate:
                    if (marketplaceDefaultsDto.Metadata.TryGetValue("ProductCommissionVatRate", out object commRateObj))
                        return Convert.ToDecimal(commRateObj);
                    else
                        return 0;
                case ProductExpenseType.ShippingCost:
                    return _taxSettings.ShippingVatRate;

                default:
                    return 0;
            }
        }

    }
}
