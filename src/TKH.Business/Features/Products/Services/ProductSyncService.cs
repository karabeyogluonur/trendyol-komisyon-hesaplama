using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Business.Integrations.Marketplaces.Factories;
using TKH.Core.Common.Constants;
using TKH.Core.Common.Settings;
using TKH.Core.DataAccess;
using TKH.Entities;
using TKH.Entities.Enums;

namespace TKH.Business.Features.Products.Services
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

        #region Sync Products From Marketplace

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

                List<string> incomingMarketplaceProductIdList = marketplaceProductDtoList
                    .Select(product => product.ExternalId)
                    .Where(id => !string.IsNullOrEmpty(id))
                    .ToList();

                List<int> incomingCategoryIdList = marketplaceProductDtoList
                    .Select(product => product.ExternalCategoryId)
                    .Distinct()
                    .ToList();


                List<string> incomingCategoryIdStringList = incomingCategoryIdList.Select(id => id.ToString()).ToList();

                IList<Category> relatedCategoryList = await scopedCategoryRepository.GetAllAsync(
                    predicate: category => incomingCategoryIdStringList.Contains(category.ExternalId),
                    include: source => source.Include(category => category.Attributes)
                                             .ThenInclude(categoryAttribute => categoryAttribute.Values),
                    disableTracking: true,
                    ignoreQueryFilters: true
                );

                IList<Product> existingProductList = await scopedProductRepository.GetAllAsync(
                    predicate: product => product.MarketplaceAccountId == marketplaceAccountId && incomingMarketplaceProductIdList.Contains(product.ExternalId),
                    include: source => source.Include(product => product.Attributes)
                                             .Include(product => product.Prices)
                                             .Include(product => product.Expenses),
                    disableTracking: false,
                    ignoreQueryFilters: true
                );

                List<Product> newProductsToAdd = new List<Product>();

                foreach (MarketplaceProductDto marketplaceProductDto in marketplaceProductDtoList)
                {
                    Product? existingProduct = existingProductList.FirstOrDefault(product => product.ExternalId == marketplaceProductDto.ExternalId);
                    Category? matchedCategory = relatedCategoryList.FirstOrDefault(category => category.ExternalId == marketplaceProductDto.ExternalCategoryId.ToString());

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

            SyncProductPrices(product, dto.Prices);
            SyncProductExpenses(product, dto.Expenses);

            if (matchedCategory is not null)
            {
                product.CategoryId = matchedCategory.Id;
                SyncProductAttributes(product, dto.Attributes, matchedCategory);
            }
        }

        private void SyncProductPrices(Product product, List<MarketplaceProductPriceDto> incomingPrices)
        {
            if (incomingPrices is null || incomingPrices.Count is 0) return;

            if (product.Prices is null)
                product.Prices = new List<ProductPrice>();

            foreach (MarketplaceProductPriceDto incomingPriceDto in incomingPrices)
            {
                ProductPrice? activeProductPrice = product.Prices.FirstOrDefault(productPrice => productPrice.Type == incomingPriceDto.Type && productPrice.EndDate == null);

                if (activeProductPrice is not null)
                {
                    if (activeProductPrice.Amount == incomingPriceDto.Amount)
                        continue;

                    activeProductPrice.EndDate = DateTime.UtcNow;

                    product.Prices.Add(new ProductPrice
                    {
                        Type = incomingPriceDto.Type,
                        Amount = incomingPriceDto.Amount,
                        IsVatIncluded = incomingPriceDto.IsVatIncluded,
                        StartDate = DateTime.UtcNow,
                        EndDate = null
                    });
                }
                else
                {
                    product.Prices.Add(new ProductPrice
                    {
                        Type = incomingPriceDto.Type,
                        Amount = incomingPriceDto.Amount,
                        IsVatIncluded = incomingPriceDto.IsVatIncluded,
                        StartDate = DateTime.UtcNow,
                        EndDate = null
                    });
                }
            }
        }

        private void SyncProductExpenses(Product product, List<MarketplaceProductExpenseDto> incomingExpenses)
        {
            if (incomingExpenses is null || incomingExpenses.Count is 0) return;

            if (product.Expenses is null)
                product.Expenses = new List<ProductExpense>();

            foreach (MarketplaceProductExpenseDto incomingExpenseDto in incomingExpenses)
            {
                ProductExpense? activeProductExpense = product.Expenses.FirstOrDefault(productExpense => productExpense.Type == incomingExpenseDto.Type && productExpense.EndDate is null && productExpense.GenerationType == GenerationType.Automated);

                if (activeProductExpense is not null)
                {
                    if (activeProductExpense.Amount == incomingExpenseDto.Amount &&
                        activeProductExpense.VatRate == incomingExpenseDto.VatRate &&
                        activeProductExpense.IsVatIncluded == incomingExpenseDto.IsVatIncluded)
                        continue;

                    activeProductExpense.EndDate = DateTime.UtcNow;

                    product.Expenses.Add(new ProductExpense
                    {
                        Type = incomingExpenseDto.Type,
                        Amount = incomingExpenseDto.Amount,
                        VatRate = incomingExpenseDto.VatRate,
                        IsVatIncluded = incomingExpenseDto.IsVatIncluded,
                        GenerationType = GenerationType.Automated,
                        StartDate = DateTime.UtcNow,
                        EndDate = null
                    });
                }
                else
                {
                    product.Expenses.Add(new ProductExpense
                    {
                        Type = incomingExpenseDto.Type,
                        Amount = incomingExpenseDto.Amount,
                        VatRate = incomingExpenseDto.VatRate,
                        IsVatIncluded = incomingExpenseDto.IsVatIncluded,
                        GenerationType = GenerationType.Automated,
                        StartDate = DateTime.UtcNow,
                        EndDate = null
                    });
                }
            }
        }

        private void SyncProductAttributes(Product product, List<MarketplaceProductAttributeDto> incomingAttributes, Category matchedCategory)
        {
            if (incomingAttributes is null || incomingAttributes.Count == 0 || matchedCategory is null)
                return;

            if (product.Attributes == null)
                product.Attributes = new List<ProductAttribute>();

            foreach (MarketplaceProductAttributeDto incomingAttributeDto in incomingAttributes)
            {
                CategoryAttribute? matchedCategoryAttribute = matchedCategory.Attributes
                    .FirstOrDefault(categoryAttribute => categoryAttribute.ExternalId == incomingAttributeDto.ExternalAttributeId);

                if (matchedCategoryAttribute is null) continue;

                AttributeValue? matchedAttributeValue = matchedCategoryAttribute.Values
                    .FirstOrDefault(attributeValue => attributeValue.ExternalId == incomingAttributeDto.ExternalValueId);

                ProductAttribute? existingProductAttribute = product.Attributes
                    .FirstOrDefault(productAttribute => productAttribute.CategoryAttributeId == matchedCategoryAttribute.Id);

                if (existingProductAttribute is not null)
                {
                    existingProductAttribute.AttributeValueId = matchedAttributeValue?.Id;
                    existingProductAttribute.CustomValue = matchedAttributeValue is null ? incomingAttributeDto.Value : null;
                }
                else
                {
                    product.Attributes.Add(new ProductAttribute
                    {
                        CategoryAttributeId = matchedCategoryAttribute.Id,
                        AttributeValueId = matchedAttributeValue?.Id,
                        CustomValue = matchedAttributeValue is null ? incomingAttributeDto.Value : null
                    });
                }
            }
        }

        #endregion
    }
}
