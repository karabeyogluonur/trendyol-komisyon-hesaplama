using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TKH.Core.Common.Constants;
using TKH.Core.DataAccess;
using TKH.Entities;
using TKH.Entities.Enums;

namespace TKH.Business.Features.ProductExpenses.Services
{
    public class ProductExpenseSyncService : IProductExpenseSyncService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ProductExpenseSyncService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task CalculateAndSyncShippingCostsAsync()
        {
            List<(int ProductId, decimal AverageCost)> calculatedCosts = new List<(int ProductId, decimal AverageCost)>();

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<Order> orderRepository = unitOfWork.GetRepository<Order>();
                IRepository<OrderItem> orderItemRepository = unitOfWork.GetRepository<OrderItem>();
                IRepository<ShipmentTransaction> shipmentTransactionRepository = unitOfWork.GetRepository<ShipmentTransaction>();
                IRepository<Claim> claimRepository = unitOfWork.GetRepository<Claim>();

                DateTime analysisStartDate = DateTime.UtcNow.AddDays(-ApplicationDefaults.ShippingCostAnalysisLookbackDays);

                IList<Order> orders = await orderRepository.GetAllAsync(
                    predicate: order => order.OrderDate >= analysisStartDate &&
                                   (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered) &&
                                   !order.IsMicroExport,
                    disableTracking: true,
                    ignoreQueryFilters: true
                );

                List<int> orderIds = orders.Select(order => order.Id).ToList();
                List<string> orderExternalNumbers = orders.Select(order => order.ExternalOrderNumber).ToList();

                IList<OrderItem> orderItems = await orderItemRepository.GetAllAsync(
                    predicate: orderItem => orderIds.Contains(orderItem.OrderId),
                    disableTracking: true,
                    ignoreQueryFilters: true
                );

                IList<Claim> claims = await claimRepository.GetAllAsync(
                    predicate: claim => orderExternalNumbers.Contains(claim.ExternalOrderNumber),
                    disableTracking: true,
                    ignoreQueryFilters: true
                );

                HashSet<string> claimedOrderNumbers = claims.Select(claim => claim.ExternalOrderNumber).ToHashSet();

                IList<ShipmentTransaction> shipmentTransactions = await shipmentTransactionRepository.GetAllAsync(
                    predicate: transaction => orderExternalNumbers.Contains(transaction.ExternalOrderNumber)
                                           && transaction.Amount > ApplicationDefaults.MinimumShippingCostThreshold,
                    disableTracking: true,
                    ignoreQueryFilters: true
                );

                HashSet<int> singleItemOrderIds = orderItems
                    .GroupBy(orderItem => orderItem.OrderId)
                    .Where(group => group.Select(item => item.ProductId).Distinct().Count() == 1 && group.Sum(item => item.Quantity) == 1)
                    .Select(group => group.Key)
                    .ToHashSet();

                var analysisData = orders
                    .Join(orderItems,
                        order => order.Id,
                        orderItem => orderItem.OrderId,
                        (order, orderItem) => new { Order = order, OrderItem = orderItem })
                    .Join(shipmentTransactions,
                        combined => new { combined.Order.ExternalOrderNumber, combined.Order.MarketplaceAccountId },
                        transaction => new { transaction.ExternalOrderNumber, transaction.MarketplaceAccountId },
                        (combined, transaction) => new
                        {
                            combined.Order,
                            combined.OrderItem,
                            Transaction = transaction
                        })
                    .Where(data => singleItemOrderIds.Contains(data.Order.Id)
                                && !claimedOrderNumbers.Contains(data.Order.ExternalOrderNumber)
                                && data.OrderItem.ProductId != null)
                    .Select(data => new
                    {
                        ProductId = data.OrderItem.ProductId!.Value,
                        Cost = data.Transaction.Amount
                    })
                    .ToList();

                var groupedCosts = analysisData.GroupBy(data => data.ProductId).Select(group => new
                {
                    ProductId = group.Key,
                    AverageCost = Math.Round(group.Average(item => item.Cost), 2)
                }).ToList();

                foreach (var item in groupedCosts)
                    calculatedCosts.Add((item.ProductId, item.AverageCost));
            }

            if (calculatedCosts.Count > 0)
                await ProcessExpenseUpdateBatchAsync(calculatedCosts);
        }

        private async Task ProcessExpenseUpdateBatchAsync(List<(int ProductId, decimal AverageCost)> costsToSync)
        {
            for (int i = 0; i < costsToSync.Count; i += ApplicationDefaults.ExpenseSyncBatchSize)
            {
                List<(int ProductId, decimal AverageCost)> batch = costsToSync.Skip(i).Take(ApplicationDefaults.ExpenseSyncBatchSize).ToList();
                List<int> batchProductIds = batch.Select(item => item.ProductId).ToList();

                using (IServiceScope scope = _serviceScopeFactory.CreateScope())
                {
                    IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    IRepository<Product> productRepository = unitOfWork.GetRepository<Product>();

                    IList<Product> products = await productRepository.GetAllAsync(
                        predicate: product => batchProductIds.Contains(product.Id),
                        include: source => source.Include(product => product.Expenses),
                        disableTracking: false,
                        ignoreQueryFilters: true
                    );

                    foreach ((int ProductId, decimal AverageCost) item in batch)
                    {
                        Product? product = products.FirstOrDefault(product => product.Id == item.ProductId);

                        if (product is null) continue;

                        if (product.Expenses is null)
                            product.Expenses = new List<ProductExpense>();

                        ProductExpense? activeExpense = product.Expenses.FirstOrDefault(expense => expense.Type == ProductExpenseType.ShippingCost && expense.EndDate == null);

                        if (activeExpense is not null && activeExpense.Amount == item.AverageCost)
                            continue;

                        if (activeExpense is not null)
                            activeExpense.EndDate = DateTime.UtcNow;

                        product.Expenses.Add(new ProductExpense
                        {
                            ProductId = product.Id,
                            Type = ProductExpenseType.ShippingCost,
                            Amount = item.AverageCost,
                            IsVatIncluded = true,
                            VatRate = 20,
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
