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
                ProductPrice? activeProductPrice = product.Prices.FirstOrDefault(productPrice => productPrice.Type == incomingPriceDto.Type && productPrice.EndDate == null && productPrice.GenerationType == GenerationType.Automated);

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
                        GenerationType = GenerationType.Automated,
                        EndDate = null
                    });
                }
                else
                {
                    product.Prices.Add(new ProductPrice
                    {
                        Type = incomingPriceDto.Type,
                        Amount = incomingPriceDto.Amount,
                        GenerationType = GenerationType.Automated,
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


        public async Task CalculateAndSyncCommissionRatesAsync()
        {
            List<(int ProductId, decimal LatestCommissionRate)> calculatedCommissions = new List<(int ProductId, decimal LatestCommissionRate)>();

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<Order> orderRepository = unitOfWork.GetRepository<Order>();
                IRepository<OrderItem> orderItemRepository = unitOfWork.GetRepository<OrderItem>();

                DateTime analysisStartDate = DateTime.UtcNow.AddDays(-ApplicationDefaults.ProductCommissionRateAnalysisLookbackDays);

                IList<Order> orders = await orderRepository.GetAllAsync(
                    predicate: order => order.OrderDate >= analysisStartDate && (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered),
                    disableTracking: true,
                    ignoreQueryFilters: true
                );

                if (!orders.Any()) return;

                List<int> orderIds = orders.Select(order => order.Id).ToList();

                IList<OrderItem> orderItems = await orderItemRepository.GetAllAsync(
                    predicate: orderItem => orderIds.Contains(orderItem.OrderId) && orderItem.ProductId.HasValue,
                    disableTracking: true,
                    ignoreQueryFilters: true
                );

                var analysisData = orderItems
                    .Join(orders, item => item.OrderId, order => order.Id, (item, order) => new { Item = item, Order = order })
                    .GroupBy(order => order.Item.ProductId!.Value)
                    .Select(group => new
                    {
                        ProductId = group.Key,
                        LatestCommissionRate = group.OrderByDescending(ordergroup => ordergroup.Order.OrderDate)
                                                    .Select(order => order.Item.CommissionRate)
                                                    .FirstOrDefault()
                    })
                    .ToList();

                foreach (var item in analysisData)
                    calculatedCommissions.Add((item.ProductId, item.LatestCommissionRate));
            }

            if (calculatedCommissions.Count > 0)
                await ProcessCommissionUpdateBatchAsync(calculatedCommissions);
        }

        private async Task ProcessCommissionUpdateBatchAsync(List<(int ProductId, decimal LatestCommissionRate)> commissionsToSync)
        {
            for (int i = 0; i < commissionsToSync.Count; i += ApplicationDefaults.ExpenseSyncBatchSize)
            {
                var batch = commissionsToSync.Skip(i).Take(ApplicationDefaults.ExpenseSyncBatchSize).ToList();
                var batchProductIds = batch.Select(item => item.ProductId).ToList();

                using (IServiceScope scope = _serviceScopeFactory.CreateScope())
                {
                    IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    TaxSettings taxSettings = scope.ServiceProvider.GetRequiredService<TaxSettings>();

                    IRepository<Product> productRepository = unitOfWork.GetRepository<Product>();

                    IList<Product> products = await productRepository.GetAllAsync(
                        predicate: product => batchProductIds.Contains(product.Id),
                        include: source => source.Include(product => product.Expenses.Where(productexpense => productexpense.GenerationType == GenerationType.Automated)),
                        disableTracking: false,
                        ignoreQueryFilters: true
                    );

                    foreach (var (productId, latestRate) in batch)
                    {
                        Product? product = products.FirstOrDefault(product => product.Id == productId);

                        if (product is null) continue;

                        if (product.Expenses is null) product.Expenses = new List<ProductExpense>();

                        ProductExpense? activeExpense = product.Expenses.FirstOrDefault(expense => expense.Type == ProductExpenseType.CommissionRate && expense.EndDate is null);

                        if (activeExpense is not null && activeExpense.Amount == latestRate)
                            continue;

                        if (activeExpense is not null)
                            activeExpense.EndDate = DateTime.UtcNow;

                        product.Expenses.Add(new ProductExpense
                        {
                            ProductId = product.Id,
                            Type = ProductExpenseType.CommissionRate,
                            GenerationType = GenerationType.Automated,
                            Amount = latestRate,
                            IsVatIncluded = true,
                            VatRate = taxSettings.ComissionVatRate,
                            StartDate = DateTime.UtcNow,
                            EndDate = null
                        });
                    }

                    await unitOfWork.SaveChangesAsync();
                }
            }
        }
    }
}
