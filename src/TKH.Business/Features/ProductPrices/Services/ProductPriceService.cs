using TKH.Business.Features.ProductPrices.Models;
using TKH.Business.Features.ProductPrices.Services;
using TKH.Business.Features.Products.Dtos;
using TKH.Business.Features.Products.Services;
using TKH.Core.DataAccess;
using TKH.Core.Utilities.Results;
using TKH.Entities;

namespace TKH.Business.Features.ProductPrices.Services
{
    public class ProductPriceService : IProductPriceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ProductPrice> _productPriceRepository;
        private readonly IProductService _productService;

        public ProductPriceService(IUnitOfWork unitOfWork, IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _productPriceRepository = _unitOfWork.GetRepository<ProductPrice>();
            _productService = productService;
        }

        public async Task<IResult> AddAsync(ProductPriceAddDto productPriceAddDto)
        {
            IDataResult<ProductSummaryDto> getProductResult = await _productService.GetByIdAsync(productPriceAddDto.ProductId);

            if (!getProductResult.Success)
                return new ErrorResult(getProductResult.Message ?? "Ürün bulunamadı.");

            ProductPrice activePrice = await _productPriceRepository.GetFirstOrDefaultAsync(predicate: product => product.ProductId == productPriceAddDto.ProductId && product.Type == productPriceAddDto.Type && product.EndDate == null);

            if (activePrice is not null)
            {
                if (activePrice.Amount == productPriceAddDto.Amount)
                    return new SuccessResult("Girilen fiyat mevcut aktif fiyat ile aynı, işlem yapılmadı.");

                activePrice.EndDate = DateTime.UtcNow;
                _productPriceRepository.Update(activePrice);
            }

            ProductPrice newPrice = new ProductPrice
            {
                ProductId = productPriceAddDto.ProductId,
                Type = productPriceAddDto.Type,
                Amount = productPriceAddDto.Amount,
                IsVatIncluded = true,
                StartDate = DateTime.UtcNow,
                EndDate = null,
            };

            await _productPriceRepository.InsertAsync(newPrice);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Ürün fiyatı başarıyla güncellendi.");
        }

        public async Task<IResult> AddRangeAsync(List<ProductPriceAddDto> productPriceAddDtos)
        {
            if (productPriceAddDtos is null || !productPriceAddDtos.Any())
                return new SuccessResult("İşlenecek kayıt bulunamadı.");

            List<int> productIds = productPriceAddDtos.Select(product => product.ProductId).Distinct().ToList();

            IDataResult<List<ProductSummaryDto>> getProductsResult = await _productService.GetByIdsAsync(productIds);

            if (!getProductsResult.Success)
                return new ErrorResult("Ürün bilgileri alınırken hata oluştu.");

            List<int> foundedProductIds = getProductsResult.Data.Select(product => product.Id).ToList();

            List<ProductPriceAddDto> foundedProductPriceAddDtos = productPriceAddDtos.Where(dto => foundedProductIds.Contains(dto.ProductId)).ToList();

            if (!foundedProductPriceAddDtos.Any())
                return new ErrorResult("İşlenecek geçerli ürün bulunamadı.");

            IList<ProductPrice> existingPrices = await _productPriceRepository.GetAllAsync(predicate: productPrice => productIds.Contains(productPrice.ProductId) && productPrice.EndDate == null, disableTracking: false);

            bool anyChanges = false;

            foreach (var foundedProductPriceAddDto in foundedProductPriceAddDtos)
            {
                ProductPrice? existingPrice = existingPrices.FirstOrDefault(productPrice => productPrice.ProductId == foundedProductPriceAddDto.ProductId && productPrice.Type == foundedProductPriceAddDto.Type);

                if (existingPrice is not null && existingPrice.Amount == foundedProductPriceAddDto.Amount)
                    continue;

                if (existingPrice is not null)
                {
                    existingPrice.EndDate = DateTime.UtcNow;
                    _productPriceRepository.Update(existingPrice);
                }

                ProductPrice newPrice = new ProductPrice
                {
                    ProductId = foundedProductPriceAddDto.ProductId,
                    Type = foundedProductPriceAddDto.Type,
                    Amount = foundedProductPriceAddDto.Amount,
                    IsVatIncluded = true,
                    StartDate = DateTime.UtcNow,
                    EndDate = null
                };

                await _productPriceRepository.InsertAsync(newPrice);
                anyChanges = true;
            }

            if (anyChanges)
            {
                await _unitOfWork.SaveChangesAsync();
                return new SuccessResult($"{foundedProductPriceAddDtos.Count} adet fiyat kaydı güncellendi/eklendi.");
            }

            return new SuccessResult("Tüm kayıtlar zaten güncel, değişiklik yapılmadı.");
        }
    }
}
