using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Business.Integrations.Marketplaces.Factories;
using TKH.Core.Common.Constants;
using TKH.Core.DataAccess;
using TKH.Entities;
using TKH.Entities.Enums;

namespace TKH.Business.Features.Products.Services
{
    public class ProductSyncService : IProductSyncService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;
        private readonly ILogger<ProductSyncService> _logger;

        public ProductSyncService(
            IServiceScopeFactory serviceScopeFactory,
            MarketplaceProviderFactory marketplaceProviderFactory,
            ILogger<ProductSyncService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _marketplaceProviderFactory = marketplaceProviderFactory;
            _logger = logger;
        }

        #region Sync Products From Marketplace

        public async Task SyncProductsFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto)
        {
            _logger.LogInformation("Starting product sync for MarketplaceAccount: {AccountId}", marketplaceAccountConnectionDetailsDto.Id);

            IMarketplaceProductProvider marketplaceProductProvider = _marketplaceProviderFactory.GetProvider<IMarketplaceProductProvider>(marketplaceAccountConnectionDetailsDto.MarketplaceType);

            List<MarketplaceProductDto> marketplaceProductDtoBufferList = new List<MarketplaceProductDto>(ApplicationDefaults.ProductBatchSize);

            await foreach (MarketplaceProductDto incomingMarketplaceProductDto in marketplaceProductProvider.GetProductsStreamAsync(marketplaceAccountConnectionDetailsDto))
            {
                marketplaceProductDtoBufferList.Add(incomingMarketplaceProductDto);

                if (marketplaceProductDtoBufferList.Count >= ApplicationDefaults.ProductBatchSize)
                {
                    await ProcessProductBatchAsync(marketplaceProductDtoBufferList, marketplaceAccountConnectionDetailsDto.Id);
                    marketplaceProductDtoBufferList.Clear();
                }
            }

            if (marketplaceProductDtoBufferList.Count > 0)
                await ProcessProductBatchAsync(marketplaceProductDtoBufferList, marketplaceAccountConnectionDetailsDto.Id);

            _logger.LogInformation("Product sync completed for MarketplaceAccount: {AccountId}", marketplaceAccountConnectionDetailsDto.Id);
        }

        private async Task ProcessProductBatchAsync(List<MarketplaceProductDto> marketplaceProductDtoList, int marketplaceAccountId)
        {
            using (IServiceScope serviceScope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork scopedUnitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<Product> scopedProductRepository = scopedUnitOfWork.GetRepository<Product>();
                IRepository<Category> scopedCategoryRepository = scopedUnitOfWork.GetRepository<Category>();

                List<string> incomingMarketplaceProductIdList = marketplaceProductDtoList
                    .Select(marketplaceProductDto => marketplaceProductDto.ExternalId)
                    .Where(externalId => !string.IsNullOrEmpty(externalId))
                    .ToList();

                List<string> incomingCategoryIdStringList = marketplaceProductDtoList
                    .Select(marketplaceProductDto => marketplaceProductDto.ExternalCategoryId.ToString())
                    .Distinct()
                    .ToList();

                IList<Category> relatedCategoryEntityList = await scopedCategoryRepository.GetAllAsync(
                    predicate: category => incomingCategoryIdStringList.Contains(category.ExternalId),
                    include: source => source.Include(category => category.Attributes)
                                             .ThenInclude(categoryAttribute => categoryAttribute.Values),
                    disableTracking: true,
                    ignoreQueryFilters: true
                );

                IList<Product> existingProductEntityList = await scopedProductRepository.GetAllAsync(
                    predicate: product => product.MarketplaceAccountId == marketplaceAccountId &&
                                          incomingMarketplaceProductIdList.Contains(product.ExternalId),
                    include: source => source.Include(product => product.Attributes)
                                             .Include(product => product.Prices)
                                             .Include(product => product.Expenses),
                    disableTracking: false,
                    ignoreQueryFilters: true
                );

                List<Product> newProductEntitiesToAddList = new List<Product>();

                foreach (MarketplaceProductDto marketplaceProductDto in marketplaceProductDtoList)
                {
                    Product? existingProductEntity = existingProductEntityList.FirstOrDefault(product => product.ExternalId == marketplaceProductDto.ExternalId);

                    Category? matchedCategoryEntity = relatedCategoryEntityList.FirstOrDefault(category => category.ExternalId == marketplaceProductDto.ExternalCategoryId.ToString());
                    int? matchedCategoryId = matchedCategoryEntity?.Id;

                    if (existingProductEntity is not null)
                    {
                        existingProductEntity.UpdateGeneralInfo(
                            marketplaceProductDto.ExternalProductCode,
                            marketplaceProductDto.Name,
                            marketplaceProductDto.Barcode,
                            marketplaceProductDto.Sku,
                            marketplaceProductDto.ModelCode,
                            marketplaceProductDto.ExternalUrl,
                            marketplaceProductDto.ImageUrl,
                            marketplaceProductDto.Deci,
                            marketplaceProductDto.VatRate,
                            marketplaceProductDto.StockQuantity,
                            marketplaceProductDto.IsOnSale,
                            DateTime.UtcNow
                        );

                        existingProductEntity.UpdateCategory(matchedCategoryId);

                        ApplyProductDetails(existingProductEntity, marketplaceProductDto, matchedCategoryEntity);
                    }
                    else
                    {
                        Product newProductEntity = Product.Create(
                            marketplaceAccountId,
                            marketplaceProductDto.ExternalId,
                            marketplaceProductDto.ExternalProductCode,
                            marketplaceProductDto.Name,
                            marketplaceProductDto.Barcode,
                            marketplaceProductDto.Sku,
                            marketplaceProductDto.ModelCode,
                            marketplaceProductDto.ExternalUrl,
                            marketplaceProductDto.ImageUrl,
                            marketplaceProductDto.Deci,
                            marketplaceProductDto.VatRate,
                            marketplaceProductDto.StockQuantity,
                            marketplaceProductDto.IsOnSale,
                            matchedCategoryId
                        );

                        ApplyProductDetails(newProductEntity, marketplaceProductDto, matchedCategoryEntity);

                        newProductEntitiesToAddList.Add(newProductEntity);
                    }
                }

                if (newProductEntitiesToAddList.Count > 0)
                    await scopedProductRepository.InsertAsync(newProductEntitiesToAddList);

                await scopedUnitOfWork.SaveChangesAsync();
            }
        }

        private void ApplyProductDetails(Product productEntity, MarketplaceProductDto marketplaceProductDto, Category? matchedCategoryEntity)
        {
            if (marketplaceProductDto.Prices is not null)
            {
                foreach (MarketplaceProductPriceDto marketplaceProductPriceDto in marketplaceProductDto.Prices)
                {
                    productEntity.AddOrUpdatePrice(
                        marketplaceProductPriceDto.Type,
                        marketplaceProductPriceDto.Amount,
                        marketplaceProductPriceDto.IsVatIncluded
                    );
                }
            }

            if (marketplaceProductDto.Expenses is not null)
            {
                foreach (MarketplaceProductExpenseDto marketplaceProductExpenseDto in marketplaceProductDto.Expenses)
                {
                    productEntity.AddOrUpdateExpense(
                        marketplaceProductExpenseDto.Type,
                        GenerationType.Automated,
                        marketplaceProductExpenseDto.Amount,
                        marketplaceProductExpenseDto.VatRate,
                        marketplaceProductExpenseDto.IsVatIncluded
                    );
                }
            }

            if (marketplaceProductDto.Attributes is not null && matchedCategoryEntity is not null)
            {
                foreach (MarketplaceProductAttributeDto marketplaceProductAttributeDto in marketplaceProductDto.Attributes)
                {
                    CategoryAttribute? matchedCategoryAttributeEntity = matchedCategoryEntity.Attributes
                        .FirstOrDefault(categoryAttribute => categoryAttribute.ExternalId == marketplaceProductAttributeDto.ExternalAttributeId);

                    if (matchedCategoryAttributeEntity is null)
                        continue;

                    AttributeValue? matchedAttributeValueEntity = matchedCategoryAttributeEntity.Values.FirstOrDefault(attributeValue => attributeValue.ExternalId == marketplaceProductAttributeDto.ExternalValueId);

                    int? attributeValueId = matchedAttributeValueEntity?.Id;

                    string? customValue = matchedAttributeValueEntity is null ? marketplaceProductAttributeDto.Value : null;

                    productEntity.AddOrUpdateAttribute(
                        matchedCategoryAttributeEntity.Id,
                        attributeValueId,
                        customValue
                    );
                }
            }
        }

        #endregion
    }
}
