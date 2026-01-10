using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TKH.Business.Common.Services;
using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Core.Common.Constants;
using TKH.Core.Common.Settings;
using TKH.Core.DataAccess;
using TKH.Entities;
using TKH.Entities.Enums;

namespace TKH.Business.Features.ProductExpenses.Services
{
    public class ProductExpenseSyncService : IProductExpenseSyncService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ProductExpenseSyncService> _logger;

        public ProductExpenseSyncService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<ProductExpenseSyncService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task CalculateAndSyncShippingCostsAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto)
        {
            _logger.LogInformation("Starting shipping cost analysis for MarketplaceAccount: {AccountId}", marketplaceAccountConnectionDetailsDto.Id);

            List<(int ProductId, decimal AverageCost)> calculatedCostsList = new List<(int ProductId, decimal AverageCost)>();

            int marketplaceAccountId = marketplaceAccountConnectionDetailsDto.Id;
            MarketplaceType marketplaceType = marketplaceAccountConnectionDetailsDto.MarketplaceType;

            using (IServiceScope serviceScope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork unitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<Order> orderRepository = unitOfWork.GetRepository<Order>();
                IRepository<OrderItem> orderItemRepository = unitOfWork.GetRepository<OrderItem>();
                IRepository<ShipmentTransaction> shipmentTransactionRepository = unitOfWork.GetRepository<ShipmentTransaction>();
                IRepository<Claim> claimRepository = unitOfWork.GetRepository<Claim>();

                DateTime analysisStartDate = DateTime.UtcNow.AddDays(-ApplicationDefaults.ShippingCostAnalysisLookbackDays);

                IList<Order> shippedOrders = await orderRepository.GetAllAsync(
                    predicate: order => order.MarketplaceAccountId == marketplaceAccountId
                                     && order.OrderDate >= analysisStartDate
                                     && (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
                                     && !order.IsMicroExport,
                    disableTracking: true,
                    ignoreQueryFilters: true
                );

                if (!shippedOrders.Any())
                {
                    _logger.LogInformation("No eligible orders found for shipping cost analysis. AccountId: {AccountId}", marketplaceAccountId);
                    return;
                }

                List<int> orderIds = shippedOrders.Select(order => order.Id).ToList();
                List<string> orderExternalNumbers = shippedOrders.Select(order => order.ExternalOrderNumber).ToList();

                IList<OrderItem> orderItems = await orderItemRepository.GetAllAsync(
                    predicate: orderItem => orderIds.Contains(orderItem.OrderId),
                    disableTracking: true,
                    ignoreQueryFilters: true
                );

                IList<Claim> existingClaims = await claimRepository.GetAllAsync(
                    predicate: claim => orderExternalNumbers.Contains(claim.ExternalOrderNumber),
                    disableTracking: true,
                    ignoreQueryFilters: true
                );

                HashSet<string> claimedOrderNumbersHashSet = existingClaims.Select(claim => claim.ExternalOrderNumber).ToHashSet();

                IList<ShipmentTransaction> shipmentTransactions = await shipmentTransactionRepository.GetAllAsync(
                    predicate: shipmentTransaction => shipmentTransaction.MarketplaceAccountId == marketplaceAccountId
                                                   && orderExternalNumbers.Contains(shipmentTransaction.ExternalOrderNumber)
                                                   && shipmentTransaction.Amount > ApplicationDefaults.MinimumShippingCostThreshold,
                    disableTracking: true,
                    ignoreQueryFilters: true
                );

                HashSet<int> singleItemOrderIdsHashSet = orderItems
                    .GroupBy(orderItem => orderItem.OrderId)
                    .Where(group => group.Select(orderItem => orderItem.ProductId).Distinct().Count() == 1 && group.Sum(orderItem => orderItem.Quantity) == 1)
                    .Select(group => group.Key)
                    .ToHashSet();

                var analysisDataList = shippedOrders
                    .Join(orderItems,
                        order => order.Id,
                        orderItem => orderItem.OrderId,
                        (order, orderItem) => new { Order = order, OrderItem = orderItem })
                    .Join(shipmentTransactions,
                        combined => combined.Order.ExternalOrderNumber,
                        shipmentTransaction => shipmentTransaction.ExternalOrderNumber,
                        (combined, shipmentTransaction) => new
                        {
                            combined.Order,
                            combined.OrderItem,
                            Transaction = shipmentTransaction
                        })
                    .Where(data => singleItemOrderIdsHashSet.Contains(data.Order.Id)
                                && !claimedOrderNumbersHashSet.Contains(data.Order.ExternalOrderNumber)
                                && data.OrderItem.ProductId != null)
                    .Select(data => new
                    {
                        ProductId = data.OrderItem.ProductId!.Value,
                        Cost = data.Transaction.Amount
                    })
                    .ToList();

                var groupedCostsList = analysisDataList
                    .GroupBy(data => data.ProductId)
                    .Select(group => new
                    {
                        ProductId = group.Key,
                        AverageCost = Math.Round(group.Average(item => item.Cost), 2)
                    }).ToList();

                foreach (var costItem in groupedCostsList)
                    calculatedCostsList.Add((costItem.ProductId, costItem.AverageCost));
            }

            if (calculatedCostsList.Count > 0)
            {
                _logger.LogInformation("Analysis completed. Updating costs for {Count} products.", calculatedCostsList.Count);
                await ProcessExpenseUpdateBatchAsync(calculatedCostsList, marketplaceType);
            }
            else
                _logger.LogInformation("Analysis completed. No cost updates required.");
        }

        public async Task CalculateAndSyncCommissionRatesAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto)
        {
            _logger.LogInformation("Starting commission rate analysis for MarketplaceAccount: {AccountId}", marketplaceAccountConnectionDetailsDto.Id);

            List<(int ProductId, decimal LatestCommissionRate)> finalCommissionsToSyncList = new List<(int ProductId, decimal LatestCommissionRate)>();

            int marketplaceAccountId = marketplaceAccountConnectionDetailsDto.Id;
            MarketplaceType marketplaceType = marketplaceAccountConnectionDetailsDto.MarketplaceType;

            using (IServiceScope serviceScope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork unitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<Order> orderRepository = unitOfWork.GetRepository<Order>();
                IRepository<OrderItem> orderItemRepository = unitOfWork.GetRepository<OrderItem>();
                IRepository<Product> productRepository = unitOfWork.GetRepository<Product>();

                DateTime analysisStartDate = DateTime.UtcNow.AddDays(-ApplicationDefaults.ProductCommissionRateAnalysisLookbackDays);
                Dictionary<int, decimal> orderBasedRatesDictionary = new Dictionary<int, decimal>();

                IList<Order> shippedOrders = await orderRepository.GetAllAsync(
                    predicate: order => order.MarketplaceAccountId == marketplaceAccountId
                                     && order.OrderDate >= analysisStartDate
                                     && (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered),
                    disableTracking: true,
                    ignoreQueryFilters: true
                );

                if (shippedOrders.Any())
                {
                    List<int> orderIds = shippedOrders.Select(order => order.Id).ToList();

                    IList<OrderItem> orderItems = await orderItemRepository.GetAllAsync(
                        predicate: orderItem => orderIds.Contains(orderItem.OrderId) && orderItem.ProductId.HasValue,
                        disableTracking: true,
                        ignoreQueryFilters: true
                    );

                    var orderAnalysisDataList = orderItems
                        .Join(shippedOrders, orderItem => orderItem.OrderId, order => order.Id, (orderItem, order) => new { Item = orderItem, Order = order })
                        .GroupBy(combined => combined.Item.ProductId!.Value)
                        .Select(group => new
                        {
                            ProductId = group.Key,
                            LatestCommissionRate = group.OrderByDescending(g => g.Order.OrderDate)
                                                        .Select(g => g.Item.CommissionRate)
                                                        .FirstOrDefault()
                        })
                        .ToList();

                    orderBasedRatesDictionary = orderAnalysisDataList.ToDictionary(k => k.ProductId, v => v.LatestCommissionRate);
                }

                var productCategoryDataList = await productRepository.GetAllAsync(
                    predicate: product => product.MarketplaceAccountId == marketplaceAccountId,
                    selector: product => new
                    {
                        ProductId = product.Id,
                        CategoryDefaultRate = product.Category != null ? product.Category.DefaultCommissionRate : null
                    },
                    include: source => source.Include(product => product.Category),
                    disableTracking: true,
                    ignoreQueryFilters: true
                );

                foreach (var productCategoryData in productCategoryDataList)
                {
                    decimal rateToUse = 0;
                    bool isRateFound = false;

                    if (orderBasedRatesDictionary.TryGetValue(productCategoryData.ProductId, out decimal orderRate))
                    {
                        rateToUse = orderRate;
                        isRateFound = true;
                    }
                    else if (productCategoryData.CategoryDefaultRate.HasValue)
                    {
                        rateToUse = productCategoryData.CategoryDefaultRate.Value;
                        isRateFound = true;
                    }

                    if (isRateFound)
                        finalCommissionsToSyncList.Add((productCategoryData.ProductId, rateToUse));
                }
            }

            if (finalCommissionsToSyncList.Count > 0)
            {
                _logger.LogInformation("Commission analysis completed. Updating rates for {Count} products.", finalCommissionsToSyncList.Count);
                await ProcessCommissionUpdateBatchAsync(finalCommissionsToSyncList, marketplaceType);
            }
            else
                _logger.LogInformation("Commission analysis completed. No updates required.");
        }

        private async Task ProcessExpenseUpdateBatchAsync(List<(int ProductId, decimal AverageCost)> costsToSyncList, MarketplaceType marketplaceType)
        {
            for (int index = 0; index < costsToSyncList.Count; index += ApplicationDefaults.ExpenseSyncBatchSize)
            {
                var currentBatchList = costsToSyncList.Skip(index).Take(ApplicationDefaults.ExpenseSyncBatchSize).ToList();
                var batchProductIds = currentBatchList.Select(item => item.ProductId).ToList();

                using (IServiceScope serviceScope = _serviceScopeFactory.CreateScope())
                {
                    IUnitOfWork unitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    IRepository<Product> productRepository = unitOfWork.GetRepository<Product>();
                    TaxSettings taxSettings = serviceScope.ServiceProvider.GetRequiredService<TaxSettings>();

                    IList<Product> products = await productRepository.GetAllAsync(
                        predicate: product => batchProductIds.Contains(product.Id),
                        include: source => source.Include(product => product.Expenses.Where(expense => expense.GenerationType == GenerationType.Automated)),
                        disableTracking: false,
                        ignoreQueryFilters: true
                    );

                    foreach ((int productId, decimal averageCost) in currentBatchList)
                    {
                        Product? product = products.FirstOrDefault(product => product.Id == productId);

                        if (product is null) continue;

                        product.AddOrUpdateExpense(
                            ProductExpenseType.ShippingCost,
                            averageCost,
                            taxSettings.ShippingVatRate,
                            isVatIncluded: true
                        );
                    }

                    await unitOfWork.SaveChangesAsync();
                }
            }
        }

        private async Task ProcessCommissionUpdateBatchAsync(List<(int ProductId, decimal LatestCommissionRate)> commissionsToSyncList, MarketplaceType marketplaceType)
        {
            for (int index = 0; index < commissionsToSyncList.Count; index += ApplicationDefaults.ExpenseSyncBatchSize)
            {
                var currentBatchList = commissionsToSyncList.Skip(index).Take(ApplicationDefaults.ExpenseSyncBatchSize).ToList();
                var batchProductIds = currentBatchList.Select(item => item.ProductId).ToList();

                using (IServiceScope serviceScope = _serviceScopeFactory.CreateScope())
                {
                    IUnitOfWork unitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    IRepository<Product> productRepository = unitOfWork.GetRepository<Product>();
                    IMarketplaceTaxService marketplaceTaxService = serviceScope.ServiceProvider.GetRequiredService<IMarketplaceTaxService>();

                    IList<Product> products = await productRepository.GetAllAsync(
                        predicate: product => batchProductIds.Contains(product.Id),
                        include: source => source.Include(product => product.Expenses.Where(expense => expense.GenerationType == GenerationType.Automated)),
                        disableTracking: false,
                        ignoreQueryFilters: true
                    );

                    foreach (var (productId, latestRate) in currentBatchList)
                    {
                        Product? product = products.FirstOrDefault(product => product.Id == productId);

                        if (product is null) continue;

                        decimal vatRate = marketplaceTaxService.GetVatRateByExpenseType(marketplaceType, ProductExpenseType.CommissionRate);

                        product.AddOrUpdateExpense(
                            ProductExpenseType.CommissionRate,
                            latestRate,
                            vatRate,
                            isVatIncluded: true
                        );
                    }

                    await unitOfWork.SaveChangesAsync();
                }
            }
        }
    }
}
