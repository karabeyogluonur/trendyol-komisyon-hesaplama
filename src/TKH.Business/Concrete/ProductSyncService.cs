using AutoMapper;
using TKH.Business.Abstract;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Business.Integrations.Abstract;
using TKH.Business.Integrations.Dtos;
using TKH.Business.Integrations.Factories;
using TKH.Core.Common.Constants;
using TKH.Core.DataAccess;
using TKH.Entities;

namespace TKH.Business.Concrete
{
    public class ProductSyncService : IProductSyncService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;
        private readonly IMapper _mapper;

        public ProductSyncService(
            IUnitOfWork unitOfWork,
            MarketplaceProviderFactory marketplaceProviderFactory,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _productRepository = _unitOfWork.GetRepository<Product>();
            _categoryRepository = _unitOfWork.GetRepository<Category>();
            _marketplaceProviderFactory = marketplaceProviderFactory;
            _mapper = mapper;
        }

        public async Task SyncProductsFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto)
        {
            IMarketplaceProductProvider marketplaceProductProvider = _marketplaceProviderFactory.GetProvider<IMarketplaceProductProvider>(marketplaceAccountConnectionDetailsDto.MarketplaceType);

            List<MarketplaceProductDto> marketplaceProductDtoBuffer = new List<MarketplaceProductDto>(ApplicationDefaults.ProductBatchSize);

            await foreach (MarketplaceProductDto incomingProductDto in marketplaceProductProvider.GetProductsStreamAsync(marketplaceAccountConnectionDetailsDto))
            {
                marketplaceProductDtoBuffer.Add(incomingProductDto);

                if (marketplaceProductDtoBuffer.Count >= ApplicationDefaults.ProductBatchSize)
                {
                    await ProcessProductBatchAsync(marketplaceProductDtoBuffer, marketplaceAccountConnectionDetailsDto.Id);
                    marketplaceProductDtoBuffer.Clear();
                }
            }

            if (marketplaceProductDtoBuffer.Count > 0)
                await ProcessProductBatchAsync(marketplaceProductDtoBuffer, marketplaceAccountConnectionDetailsDto.Id);
        }

        private async Task ProcessProductBatchAsync(List<MarketplaceProductDto> marketplaceProductDtoList, int marketplaceAccountId)
        {
            List<string> incomingBarcodeList = marketplaceProductDtoList
                .Select(marketplaceProductDto => marketplaceProductDto.Barcode)
                .Where(barcode => !string.IsNullOrEmpty(barcode))
                .ToList();

            List<string> incomingCategoryIdList = marketplaceProductDtoList
                .Select(marketplaceProductDto => marketplaceProductDto.MarketplaceCategoryId)
                .Distinct()
                .ToList();

            List<Category> relatedCategoryList = await _categoryRepository.GetAllAsync(category => incomingCategoryIdList.Contains(category.MarketplaceCategoryId));

            List<Product> existingProductList = await _productRepository.GetAllAsync(product => product.MarketplaceAccountId == marketplaceAccountId && incomingBarcodeList.Contains(product.Barcode),
            includes: include => include.ProductAttributes);

            List<Product> productsToProcessList = new List<Product>();

            foreach (MarketplaceProductDto marketplaceProductDto in marketplaceProductDtoList)
            {
                Product existingProduct = existingProductList.FirstOrDefault(product => product.Barcode == marketplaceProductDto.Barcode);
                Category matchedCategory = relatedCategoryList.FirstOrDefault(category => category.MarketplaceCategoryId == marketplaceProductDto.MarketplaceCategoryId);

                if (existingProduct != null)
                {
                    _mapper.Map(marketplaceProductDto, existingProduct);

                    existingProduct.LastUpdateDateTime = DateTime.UtcNow;

                    if (matchedCategory != null)
                    {
                        existingProduct.CategoryId = matchedCategory.Id;
                        existingProduct.CommissionRate = matchedCategory.DefaultCommissionRate ?? 0;
                    }
                    else
                        existingProduct.CommissionRate = 0;

                    SyncProductAttributes(existingProduct, marketplaceProductDto.Attributes);
                    productsToProcessList.Add(existingProduct);
                }
                else
                {
                    Product newProduct = _mapper.Map<Product>(marketplaceProductDto);
                    newProduct.MarketplaceAccountId = marketplaceAccountId;
                    newProduct.LastUpdateDateTime = DateTime.UtcNow;

                    if (matchedCategory != null)
                    {
                        newProduct.CategoryId = matchedCategory.Id;
                        newProduct.CommissionRate = matchedCategory.DefaultCommissionRate ?? 0;
                    }
                    else
                        newProduct.CommissionRate = 0;

                    SyncProductAttributes(newProduct, marketplaceProductDto.Attributes);
                    productsToProcessList.Add(newProduct);
                }
            }

            _productRepository.AddOrUpdate(productsToProcessList);
            await _unitOfWork.SaveChangesAsync();
        }

        private void SyncProductAttributes(Product product, List<MarketplaceProductAttributeDto> incomingAttributes)
        {
            if (incomingAttributes == null || incomingAttributes.Count == 0)
                return;

            foreach (MarketplaceProductAttributeDto marketplaceProductAttributeDto in incomingAttributes)
            {
                ProductAttribute existingProductAttribute = product.ProductAttributes
                    .FirstOrDefault(productAttribute => productAttribute.MarketplaceAttributeId == marketplaceProductAttributeDto.MarketplaceAttributeId);

                if (existingProductAttribute != null)
                {
                    existingProductAttribute.AttributeName = marketplaceProductAttributeDto.AttributeName;
                    existingProductAttribute.MarketplaceAttributeValueId = marketplaceProductAttributeDto.MarketplaceValueId;
                    existingProductAttribute.Value = marketplaceProductAttributeDto.Value;
                }
                else
                {
                    ProductAttribute newProductAttribute = new ProductAttribute
                    {
                        MarketplaceAttributeId = marketplaceProductAttributeDto.MarketplaceAttributeId,
                        AttributeName = marketplaceProductAttributeDto.AttributeName,
                        MarketplaceAttributeValueId = marketplaceProductAttributeDto.MarketplaceValueId,
                        Value = marketplaceProductAttributeDto.Value
                    };
                    product.ProductAttributes.Add(newProductAttribute);
                }
            }
        }
    }
}
