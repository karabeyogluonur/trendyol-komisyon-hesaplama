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

                List<string> incomingMarketplaceProductIdList = marketplaceProductDtoList
                    .Select(product => product.MarketplaceProductId)
                    .Where(id => !string.IsNullOrEmpty(id))
                    .ToList();

                List<int> incomingCategoryIdList = marketplaceProductDtoList
                    .Select(product => product.MarketplaceCategoryId)
                    .Distinct()
                    .ToList();


                List<string> incomingCategoryIdStringList = incomingCategoryIdList.Select(id => id.ToString()).ToList();

                IList<Category> relatedCategoryList = await scopedCategoryRepository.GetAllAsync(
                    predicate: category => incomingCategoryIdStringList.Contains(category.MarketplaceCategoryId),
                    include: source => source.Include(category => category.CategoryAttributes)
                                             .ThenInclude(categoryAttribute => categoryAttribute.AttributeValues),
                    disableTracking: true
                );

                IList<Product> existingProductList = await scopedProductRepository.GetAllAsync(
                    predicate: product => product.MarketplaceAccountId == marketplaceAccountId && incomingMarketplaceProductIdList.Contains(product.MarketplaceProductId),
                    include: source => source.Include(product => product.ProductAttributes)
                                             .Include(product => product.ProductPrices)
                                             .Include(product => product.ProductExpenses),
                    disableTracking: false
                );

                List<Product> newProductsToAdd = new List<Product>();

                foreach (MarketplaceProductDto marketplaceProductDto in marketplaceProductDtoList)
                {
                    Product? existingProduct = existingProductList.FirstOrDefault(product => product.MarketplaceProductId == marketplaceProductDto.MarketplaceProductId);
                    Category? matchedCategory = relatedCategoryList.FirstOrDefault(category => category.MarketplaceCategoryId == marketplaceProductDto.MarketplaceCategoryId.ToString());

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
                if (product.CommissionRate == 0)
                    product.CommissionRate = matchedCategory.DefaultCommissionRate ?? 0;

                SyncProductAttributes(product, dto.Attributes, matchedCategory);
            }
        }

        private void SyncProductPrices(Product product, List<MarketplaceProductPriceDto> incomingPrices)
        {
            if (incomingPrices is null || incomingPrices.Count == 0) return;

            if (product.ProductPrices == null)
                product.ProductPrices = new List<ProductPrice>();

            foreach (MarketplaceProductPriceDto incomingPriceDto in incomingPrices)
            {
                ProductPrice? activeProductPrice = product.ProductPrices
                    .FirstOrDefault(productPrice => productPrice.Type == incomingPriceDto.Type && productPrice.EndDate == null);

                if (activeProductPrice is not null)
                {
                    if (activeProductPrice.Amount == incomingPriceDto.Amount)
                        continue;

                    activeProductPrice.EndDate = DateTime.UtcNow;

                    product.ProductPrices.Add(new ProductPrice
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
                    product.ProductPrices.Add(new ProductPrice
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
            if (incomingExpenses is null || incomingExpenses.Count == 0) return;

            if (product.ProductExpenses == null)
                product.ProductExpenses = new List<ProductExpense>();

            foreach (MarketplaceProductExpenseDto incomingExpenseDto in incomingExpenses)
            {
                ProductExpense? activeProductExpense = product.ProductExpenses
                    .FirstOrDefault(productExpense => productExpense.Type == incomingExpenseDto.Type && productExpense.EndDate == null);

                if (activeProductExpense is not null)
                {
                    if (activeProductExpense.Amount == incomingExpenseDto.Amount &&
                        activeProductExpense.VatRate == incomingExpenseDto.VatRate &&
                        activeProductExpense.IsVatIncluded == incomingExpenseDto.IsVatIncluded)
                        continue;

                    activeProductExpense.EndDate = DateTime.UtcNow;

                    product.ProductExpenses.Add(new ProductExpense
                    {
                        Type = incomingExpenseDto.Type,
                        Amount = incomingExpenseDto.Amount,
                        VatRate = incomingExpenseDto.VatRate,
                        IsVatIncluded = incomingExpenseDto.IsVatIncluded,
                        StartDate = DateTime.UtcNow,
                        EndDate = null
                    });
                }
                else
                {
                    product.ProductExpenses.Add(new ProductExpense
                    {
                        Type = incomingExpenseDto.Type,
                        Amount = incomingExpenseDto.Amount,
                        VatRate = incomingExpenseDto.VatRate,
                        IsVatIncluded = incomingExpenseDto.IsVatIncluded,
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
