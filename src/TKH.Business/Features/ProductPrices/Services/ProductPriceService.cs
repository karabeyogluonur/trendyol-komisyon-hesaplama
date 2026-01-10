using TKH.Business.Features.ProductPrices.Models;
using TKH.Business.Features.Products.Dtos;
using TKH.Business.Features.Products.Services;
using TKH.Core.DataAccess;
using TKH.Core.Utilities.Results;
using TKH.Entities;

using Microsoft.Extensions.Logging;

namespace TKH.Business.Features.ProductPrices.Services
{
    public class ProductPriceService : IProductPriceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ProductPrice> _productPriceRepository;
        private readonly IProductService _productService;
        private readonly ILogger<ProductPriceService> _logger;

        public ProductPriceService(IUnitOfWork unitOfWork, IProductService productService, ILogger<ProductPriceService> logger)
        {
            _unitOfWork = unitOfWork;
            _productPriceRepository = _unitOfWork.GetRepository<ProductPrice>();
            _productService = productService;
            _logger = logger;
        }

        public async Task<IResult> CreateProductPriceAsync(ProductPriceCreateDto productPriceCreateDto)
        {
            IDataResult<ProductSummaryDto> productResult = await _productService.GetProductByIdAsync(productPriceCreateDto.ProductId);

            if (!productResult.Success)
            {
                _logger.LogWarning("Product not found. ProductId: {ProductId}", productPriceCreateDto.ProductId);
                return new ErrorResult(productResult.Message ?? "Ürün bulunamadı.");
            }

            ProductPrice activePrice = await _productPriceRepository.GetFirstOrDefaultAsync(
                predicate: price => price.ProductId == productPriceCreateDto.ProductId && price.Type == productPriceCreateDto.Type && price.EndDate == null
            );

            if (activePrice is not null)
            {
                if (!activePrice.ShouldUpdate(productPriceCreateDto.Amount))
                    return new SuccessResult("Girilen fiyat mevcut aktif fiyat ile aynı, işlem yapılmadı.");

                activePrice.MarkAsExpired();
                _productPriceRepository.Update(activePrice);
                _logger.LogInformation("Existing price marked as expired. ProductId: {ProductId}, Type: {Type}, OldAmount: {OldAmount}", activePrice.ProductId, activePrice.Type, activePrice.Amount);
            }

            ProductPrice newPrice = ProductPrice.Create(
                productPriceCreateDto.ProductId,
                productPriceCreateDto.Type,
                productPriceCreateDto.Amount
            );

            await _productPriceRepository.InsertAsync(newPrice);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("New product price created. ProductId: {ProductId}, Type: {Type}, Amount: {Amount}", newPrice.ProductId, newPrice.Type, newPrice.Amount);

            return new SuccessResult("Ürün fiyatı başarıyla oluşturuldu.");
        }

        public async Task<IResult> CreateProductPricesAsync(List<ProductPriceCreateDto> productPriceCreateDtos)
        {
            if (productPriceCreateDtos == null || productPriceCreateDtos.Count == 0)
            {
                _logger.LogInformation("No product prices to process.");
                return new SuccessResult("İşlenecek kayıt bulunamadı.");
            }

            List<int> productIds = productPriceCreateDtos.Select(dto => dto.ProductId).Distinct().ToList();

            IDataResult<List<ProductSummaryDto>> productsResult = await _productService.GetProductsByIdsAsync(productIds);

            if (!productsResult.Success)
            {
                _logger.LogError("Failed to retrieve products for batch price creation.");
                return new ErrorResult("Ürün bilgileri alınırken hata oluştu.");
            }

            List<int> existingProductIds = productsResult.Data.Select(product => product.Id).ToList();
            List<ProductPriceCreateDto> validProductPriceCreateDtos = productPriceCreateDtos.Where(dto => existingProductIds.Contains(dto.ProductId)).ToList();

            if (validProductPriceCreateDtos.Count == 0)
            {
                _logger.LogWarning("No valid products found for price creation.");
                return new ErrorResult("İşlenecek geçerli ürün bulunamadı.");
            }

            IList<ProductPrice> activePrices = await _productPriceRepository.GetAllAsync(
                predicate: price => productIds.Contains(price.ProductId) && price.EndDate == null,
                disableTracking: false
            );

            bool anyChanges = false;

            foreach (ProductPriceCreateDto validProductPriceCreateDto in validProductPriceCreateDtos)
            {
                ProductPrice existingPrice = activePrices.FirstOrDefault(
                    price => price.ProductId == validProductPriceCreateDto.ProductId && price.Type == validProductPriceCreateDto.Type
                );

                if (existingPrice is not null && !existingPrice.ShouldUpdate(validProductPriceCreateDto.Amount))
                    continue;

                if (existingPrice is not null)
                {
                    existingPrice.MarkAsExpired();
                    _productPriceRepository.Update(existingPrice);
                    _logger.LogInformation("Existing price expired during batch creation. ProductId: {ProductId}, Type: {Type}, OldAmount: {OldAmount}", existingPrice.ProductId, existingPrice.Type, existingPrice.Amount);
                }

                ProductPrice newPrice = ProductPrice.Create(validProductPriceCreateDto.ProductId, validProductPriceCreateDto.Type, validProductPriceCreateDto.Amount);

                await _productPriceRepository.InsertAsync(newPrice);

                _logger.LogInformation("New product price added in batch. ProductId: {ProductId}, Type: {Type}, Amount: {Amount}", newPrice.ProductId, newPrice.Type, newPrice.Amount);

                anyChanges = true;
            }

            if (anyChanges)
            {
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("{Count} product prices created/updated.", validProductPriceCreateDtos.Count);
                return new SuccessResult($"{validProductPriceCreateDtos.Count} adet fiyat kaydı güncellendi/eklendi.");
            }

            return new SuccessResult("Tüm kayıtlar zaten güncel, değişiklik yapılmadı.");
        }
    }
}
