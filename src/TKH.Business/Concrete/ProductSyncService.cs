using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;
        private readonly IMapper _mapper;

        public ProductSyncService(
            IServiceScopeFactory serviceScopeFactory,
            MarketplaceProviderFactory marketplaceProviderFactory,
            IMapper mapper)
        {
            _serviceScopeFactory = serviceScopeFactory;
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
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork scopedUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<Product> scopedProductRepository = scopedUnitOfWork.GetRepository<Product>();
                IRepository<Category> scopedCategoryRepository = scopedUnitOfWork.GetRepository<Category>();

                List<string> incomingBarcodeList = marketplaceProductDtoList
                    .Select(product => product.Barcode)
                    .Where(barcode => !string.IsNullOrEmpty(barcode))
                    .ToList();

                List<string> incomingCategoryIdList = marketplaceProductDtoList
                    .Select(product => product.MarketplaceCategoryId)
                    .Distinct()
                    .ToList();

                IList<Category> relatedCategoryList = await scopedCategoryRepository.GetAllAsync(
                    predicate: category => incomingCategoryIdList.Contains(category.MarketplaceCategoryId),
                    include: source => source.Include(category => category.CategoryAttributes)
                                             .ThenInclude(categoryAttribute => categoryAttribute.AttributeValues),
                    disableTracking: true
                );

                IList<Product> existingProductList = await scopedProductRepository.GetAllAsync(
                    predicate: product => product.MarketplaceAccountId == marketplaceAccountId && incomingBarcodeList.Contains(product.Barcode),
                    include: source => source.Include(product => product.ProductAttributes),
                    disableTracking: false
                );

                List<Product> newProductsToAdd = new List<Product>();

                foreach (MarketplaceProductDto marketplaceProductDto in marketplaceProductDtoList)
                {
                    Product? existingProduct = existingProductList.FirstOrDefault(product => product.Barcode == marketplaceProductDto.Barcode);
                    Category? matchedCategory = relatedCategoryList.FirstOrDefault(category => category.MarketplaceCategoryId == marketplaceProductDto.MarketplaceCategoryId);

                    if (existingProduct is not null)
                    {
                        _mapper.Map(marketplaceProductDto, existingProduct);
                        UpdateProductDetails(existingProduct, marketplaceProductDto, matchedCategory, marketplaceAccountId);
                    }
                    else
                    {
                        Product newProduct = _mapper.Map<Product>(marketplaceProductDto);
                        UpdateProductDetails(newProduct, marketplaceProductDto, matchedCategory, marketplaceAccountId);
                        newProductsToAdd.Add(newProduct);
                    }
                }

                if (newProductsToAdd.Count > 0)
                    await scopedProductRepository.InsertAsync(newProductsToAdd);

                await scopedUnitOfWork.SaveChangesAsync();
            }
        }

        private void UpdateProductDetails(Product product, MarketplaceProductDto dto, Category? matchedCategory, int marketplaceAccountId)
        {
            product.MarketplaceAccountId = marketplaceAccountId;
            product.LastUpdateDateTime = DateTime.UtcNow;

            if (matchedCategory is not null)
            {
                product.CategoryId = matchedCategory.Id;
                product.CommissionRate = matchedCategory.DefaultCommissionRate ?? 0;
                SyncProductAttributes(product, dto.Attributes, matchedCategory);
            }
            else
                product.CommissionRate = 0;
        }

        private void SyncProductAttributes(Product product, List<MarketplaceProductAttributeDto> incomingAttributes, Category matchedCategory)
        {
            if (incomingAttributes is null || incomingAttributes.Count == 0 || matchedCategory is null)
                return;

            if (product.ProductAttributes == null)
                product.ProductAttributes = new List<ProductAttribute>();

            foreach (MarketplaceProductAttributeDto incomingAttributeDto in incomingAttributes)
            {
                CategoryAttribute? matchedCategoryAttribute = matchedCategory.CategoryAttributes
                    .FirstOrDefault(categoryAttribute => categoryAttribute.MarketplaceAttributeId == incomingAttributeDto.MarketplaceAttributeId);

                if (matchedCategoryAttribute is null) continue;

                AttributeValue? matchedAttributeValue = matchedCategoryAttribute.AttributeValues
                    .FirstOrDefault(attributeValue => attributeValue.MarketplaceValueId == incomingAttributeDto.MarketplaceValueId);

                ProductAttribute? existingProductAttribute = product.ProductAttributes
                    .FirstOrDefault(productAttribute => productAttribute.CategoryAttributeId == matchedCategoryAttribute.Id);

                if (existingProductAttribute is not null)
                {
                    existingProductAttribute.AttributeValueId = matchedAttributeValue?.Id;
                    existingProductAttribute.CustomValue = matchedAttributeValue is null ? incomingAttributeDto.Value : null;
                }
                else
                {
                    product.ProductAttributes.Add(new ProductAttribute
                    {
                        CategoryAttributeId = matchedCategoryAttribute.Id,
                        AttributeValueId = matchedAttributeValue?.Id,
                        CustomValue = matchedAttributeValue is null ? incomingAttributeDto.Value : null
                    });
                }
            }
        }
    }
}
