using TKH.Business.Features.ProductPrices.Models;
using TKH.Core.DataAccess;
using TKH.Core.Utilities.Results;
using TKH.Entities;

using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace TKH.Business.Features.ProductPrices.Services
{
    public class ProductPriceService : IProductPriceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Product> _productRepository;
        private readonly ILogger<ProductPriceService> _logger;

        public ProductPriceService(IUnitOfWork unitOfWork, ILogger<ProductPriceService> logger)
        {
            _unitOfWork = unitOfWork;
            _productRepository = _unitOfWork.GetRepository<Product>();
            _logger = logger;
        }

        public async Task<IResult> CreateProductPriceAsync(ProductPriceCreateDto productPriceCreateDto)
        {
            Product? product = await _productRepository.GetFirstOrDefaultAsync(
                predicate: product => product.Id == productPriceCreateDto.ProductId,
                include: include => include.Include(product => product.Prices),
                disableTracking: false
            );

            if (product is null)
            {
                _logger.LogWarning("Product not found for price update. ProductId: {ProductId}", productPriceCreateDto.ProductId);
                return new ErrorResult("Ürün bulunamadı.");
            }

            product.AddOrUpdatePrice(productPriceCreateDto.Type, productPriceCreateDto.Amount, isVatIncluded: true);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product price updated via Entity. ProductId: {ProductId}, Amount: {Amount}", product.Id, productPriceCreateDto.Amount);

            return new SuccessResult("Ürün fiyatı başarıyla işlendi.");
        }

        public async Task<IResult> CreateProductPricesAsync(List<ProductPriceCreateDto> productPriceCreateDtos)
        {
            if (productPriceCreateDtos is null || !productPriceCreateDtos.Any())
            {
                _logger.LogInformation("No product prices to process.");
                return new SuccessResult("İşlenecek kayıt bulunamadı.");
            }

            List<int> productIds = productPriceCreateDtos.Select(dto => dto.ProductId).Distinct().ToList();

            IList<Product> productEntities = await _productRepository.GetAllAsync(
                predicate: product => productIds.Contains(product.Id),
                include: include => include.Include(product => product.Prices),
                disableTracking: false
            );

            if (!productEntities.Any())
            {
                _logger.LogWarning("No valid products found in database for the given IDs.");
                return new ErrorResult("İşlenecek geçerli ürün bulunamadı.");
            }

            foreach (ProductPriceCreateDto productPriceCreateDto in productPriceCreateDtos)
            {
                Product product = productEntities.FirstOrDefault(product => product.Id == productPriceCreateDto.ProductId);
                if (product is null) continue;

                product.AddOrUpdatePrice(productPriceCreateDto.Type, productPriceCreateDto.Amount, isVatIncluded: true);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Batch price update completed. Processed Count: {Count}", productPriceCreateDtos.Count);
            return new SuccessResult($"{productPriceCreateDtos.Count} adet ürünün fiyat bilgisi başarıyla işlendi.");
        }
    }
}
