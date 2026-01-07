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
        private readonly IRepository<ProductExpense> _productExpenseRepository;
        private readonly IProductService _productService;
        private readonly IMarketplaceTaxService _marketplaceTaxService;
        private readonly TaxSettings _taxSettings;

        public ProductExpenseService(
            IUnitOfWork unitOfWork,
            IProductService productService,
            IMarketplaceTaxService marketplaceTaxService,
            TaxSettings taxSettings)
        {
            _unitOfWork = unitOfWork;
            _productExpenseRepository = _unitOfWork.GetRepository<ProductExpense>();
            _productService = productService;
            _marketplaceTaxService = marketplaceTaxService;
            _taxSettings = taxSettings;
        }

        public async Task<IResult> AddAsync(ProductExpenseAddDto productExpenseAddDto)
        {
            IDataResult<ProductSummaryDto> getProductResult = await _productService.GetByIdAsync(productExpenseAddDto.ProductId);

            if (!getProductResult.Success)
                return new ErrorResult(getProductResult.Message);

            ProductExpense? activeExpense = await _productExpenseRepository.GetFirstOrDefaultAsync(
                predicate: productExpense => productExpense.ProductId == productExpenseAddDto.ProductId &&
                                          productExpense.Type == productExpenseAddDto.Type &&
                                          productExpense.GenerationType == productExpenseAddDto.GenerationType &&
                                          productExpense.EndDate == null
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
                VatRate = ResolveVatRate(productExpenseAddDto.Type, getProductResult.Data.MarketplaceType)
            };

            await _productExpenseRepository.InsertAsync(newExpense);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Ürün gideri başarıyla güncellendi.");
        }

        public async Task<IResult> AddRangeAsync(List<ProductExpenseAddDto> productExpenseAddDtos)
        {
            if (productExpenseAddDtos == null || !productExpenseAddDtos.Any())
                return new SuccessResult("İşlenecek kayıt bulunamadı.");

            List<int> productIds = productExpenseAddDtos.Select(x => x.ProductId).Distinct().ToList();

            IDataResult<List<ProductSummaryDto>> getProductsResult = await _productService.GetByIdsAsync(productIds);

            if (!getProductsResult.Success)
                return new ErrorResult(getProductsResult.Message);

            List<ProductSummaryDto> products = getProductsResult.Data;
            HashSet<int> foundedProductIds = products.Select(product => product.Id).ToHashSet();
            List<ProductExpenseAddDto> foundedProductExpenseAddDtos = productExpenseAddDtos.Where(productExpense => foundedProductIds.Contains(productExpense.ProductId)).ToList();

            if (!foundedProductExpenseAddDtos.Any())
                return new ErrorResult("İşlenecek geçerli ürün bulunamadı.");

            IList<ProductExpense> existingExpenses = await _productExpenseRepository.GetAllAsync(
                predicate: productExpense => productIds.Contains(productExpense.ProductId) && productExpense.EndDate == null,
                disableTracking: false);

            Dictionary<(MarketplaceType, ProductExpenseType), decimal> vatRateCache = new Dictionary<(MarketplaceType, ProductExpenseType), decimal>();
            bool anyChanges = false;

            foreach (ProductExpenseAddDto dto in foundedProductExpenseAddDtos)
            {
                ProductExpense? existingExpense = existingExpenses.FirstOrDefault(productExpense =>
                    productExpense.ProductId == dto.ProductId &&
                    productExpense.Type == dto.Type &&
                    productExpense.GenerationType == dto.GenerationType);

                if (existingExpense is not null && existingExpense.Amount == dto.Amount)
                    continue;

                if (existingExpense is not null)
                {
                    existingExpense.EndDate = DateTime.UtcNow;
                    _productExpenseRepository.Update(existingExpense);
                }

                ProductSummaryDto product = products.First(product => product.Id == dto.ProductId);

                (MarketplaceType, ProductExpenseType) cacheKey = (product.MarketplaceType, dto.Type);

                if (!vatRateCache.TryGetValue(cacheKey, out decimal vatRate))
                {
                    vatRate = ResolveVatRate(dto.Type, product.MarketplaceType);
                    vatRateCache[cacheKey] = vatRate;
                }

                ProductExpense newExpense = new ProductExpense
                {
                    ProductId = dto.ProductId,
                    Type = dto.Type,
                    Amount = dto.Amount,
                    StartDate = DateTime.UtcNow,
                    EndDate = null,
                    GenerationType = dto.GenerationType,
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

        private decimal ResolveVatRate(ProductExpenseType expenseType, MarketplaceType marketplaceType)
        {
            switch (expenseType)
            {
                case ProductExpenseType.ShippingCost:
                    return _taxSettings.ShippingVatRate;

                default:
                    return _marketplaceTaxService.GetVatRateByExpenseType(marketplaceType, expenseType);
            }
        }
    }
}
