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

namespace TKH.Business.Features.Orders.Services
{
    public class OrderSyncService : IOrderSyncService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;
        private readonly ILogger<OrderSyncService> _logger;

        public OrderSyncService(
            IServiceScopeFactory serviceScopeFactory,
            MarketplaceProviderFactory marketplaceProviderFactory,
            ILogger<OrderSyncService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _marketplaceProviderFactory = marketplaceProviderFactory;
            _logger = logger;
        }

        public async Task SyncOrdersFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto)
        {
            _logger.LogInformation("Starting order sync for MarketplaceAccount: {AccountId}", marketplaceAccountConnectionDetailsDto.Id);

            IMarketplaceOrderProvider marketplaceOrderProvider = _marketplaceProviderFactory.GetProvider<IMarketplaceOrderProvider>(marketplaceAccountConnectionDetailsDto.MarketplaceType);

            List<MarketplaceOrderDto> marketplaceOrderDtoBufferList = new List<MarketplaceOrderDto>(ApplicationDefaults.OrderBatchSize);

            await foreach (MarketplaceOrderDto incomingMarketplaceOrderDto in marketplaceOrderProvider.GetOrdersStreamAsync(marketplaceAccountConnectionDetailsDto))
            {
                marketplaceOrderDtoBufferList.Add(incomingMarketplaceOrderDto);

                if (marketplaceOrderDtoBufferList.Count >= ApplicationDefaults.OrderBatchSize)
                {
                    await ProcessOrderBatchAsync(marketplaceOrderDtoBufferList, marketplaceAccountConnectionDetailsDto.Id);
                    marketplaceOrderDtoBufferList.Clear();
                }
            }

            if (marketplaceOrderDtoBufferList.Count > 0)
                await ProcessOrderBatchAsync(marketplaceOrderDtoBufferList, marketplaceAccountConnectionDetailsDto.Id);

            _logger.LogInformation("Order sync completed for MarketplaceAccount: {AccountId}", marketplaceAccountConnectionDetailsDto.Id);
        }

        private async Task ProcessOrderBatchAsync(List<MarketplaceOrderDto> marketplaceOrderDtoList, int marketplaceAccountId)
        {
            using (IServiceScope serviceScope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork scopedUnitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<Order> scopedOrderRepository = scopedUnitOfWork.GetRepository<Order>();
                IRepository<Product> scopedProductRepository = scopedUnitOfWork.GetRepository<Product>();

                List<string> incomingShipmentIdList = marketplaceOrderDtoList
                    .Select(marketplaceOrderDto => marketplaceOrderDto.ExternalShipmentId)
                    .Where(externalShipmentId => !string.IsNullOrEmpty(externalShipmentId))
                    .ToList();

                List<string> allMarketplaceProductCodes = marketplaceOrderDtoList
                    .SelectMany(marketplaceOrderDto => marketplaceOrderDto.Items)
                    .Select(marketplaceOrderItemDto => marketplaceOrderItemDto.ExternalProductCode)
                    .Where(externalProductCode => !string.IsNullOrEmpty(externalProductCode))
                    .Distinct()
                    .ToList();

                IList<Product> relatedProductList = await scopedProductRepository.GetAllAsync(
                    predicate: product => product.MarketplaceAccountId == marketplaceAccountId &&
                                          allMarketplaceProductCodes.Contains(product.ExternalProductCode),
                    disableTracking: true,
                    ignoreQueryFilters: true
                );

                Dictionary<string, int> productCodeToLocalIdMapDictionary = relatedProductList
                    .GroupBy(product => product.ExternalProductCode)
                    .ToDictionary(group => group.Key, group => group.First().Id);

                IList<Order> existingOrderList = await scopedOrderRepository.GetAllAsync(
                    predicate: order => order.MarketplaceAccountId == marketplaceAccountId &&
                                        incomingShipmentIdList.Contains(order.ExternalShipmentId),
                    include: source => source.Include(order => order.OrderItems),
                    disableTracking: false,
                    ignoreQueryFilters: true
                );

                List<Order> newOrdersToAddList = new List<Order>();

                foreach (MarketplaceOrderDto marketplaceOrderDto in marketplaceOrderDtoList)
                {
                    Order? existingOrderEntity = existingOrderList.FirstOrDefault(order => order.ExternalShipmentId == marketplaceOrderDto.ExternalShipmentId);

                    DateTime orderDateUtc = marketplaceOrderDto.OrderDate.Kind == DateTimeKind.Utc ? marketplaceOrderDto.OrderDate : DateTime.SpecifyKind(marketplaceOrderDto.OrderDate, DateTimeKind.Utc);

                    List<OrderItem> orderItemsForSync = CreateOrderItemsFromDto(marketplaceOrderDto.Items, productCodeToLocalIdMapDictionary);

                    if (existingOrderEntity is not null)
                    {
                        existingOrderEntity.UpdateDetails(
                            marketplaceOrderDto.GrossAmount,
                            marketplaceOrderDto.TotalDiscount,
                            marketplaceOrderDto.PlatformCoveredDiscount,
                            marketplaceOrderDto.Status,
                            marketplaceOrderDto.CargoTrackingNumber,
                            marketplaceOrderDto.CargoProviderName,
                            marketplaceOrderDto.Deci,
                            marketplaceOrderDto.IsShipmentPaidBySeller,
                            marketplaceOrderDto.IsMicroExport,
                            DateTime.UtcNow
                        );

                        existingOrderEntity.SyncOrderItems(orderItemsForSync);
                    }
                    else
                    {
                        Order newOrderEntity = Order.Create(
                            marketplaceAccountId,
                            marketplaceOrderDto.ExternalOrderNumber,
                            marketplaceOrderDto.ExternalShipmentId,
                            marketplaceOrderDto.GrossAmount,
                            marketplaceOrderDto.TotalDiscount,
                            marketplaceOrderDto.PlatformCoveredDiscount,
                            marketplaceOrderDto.CurrencyCode,
                            marketplaceOrderDto.Status,
                            orderDateUtc,
                            marketplaceOrderDto.CargoTrackingNumber,
                            marketplaceOrderDto.CargoProviderName,
                            marketplaceOrderDto.Deci,
                            marketplaceOrderDto.IsShipmentPaidBySeller,
                            marketplaceOrderDto.IsMicroExport
                        );

                        newOrderEntity.SyncOrderItems(orderItemsForSync);

                        newOrdersToAddList.Add(newOrderEntity);
                    }
                }

                if (newOrdersToAddList.Count > 0)
                    await scopedOrderRepository.InsertAsync(newOrdersToAddList);

                await scopedUnitOfWork.SaveChangesAsync();
            }
        }

        private List<OrderItem> CreateOrderItemsFromDto(List<MarketplaceOrderItemDto> marketplaceOrderItemDtos, Dictionary<string, int> productCodeToLocalIdMapDictionary)
        {
            List<OrderItem> orderItemEntities = new List<OrderItem>();

            if (marketplaceOrderItemDtos is null || !marketplaceOrderItemDtos.Any())
                return orderItemEntities;

            foreach (MarketplaceOrderItemDto marketplaceOrderItemDto in marketplaceOrderItemDtos)
            {
                int? matchedProductId = null;

                if (!string.IsNullOrEmpty(marketplaceOrderItemDto.ExternalProductCode) && productCodeToLocalIdMapDictionary.TryGetValue(marketplaceOrderItemDto.ExternalProductCode, out int productId))
                    matchedProductId = productId;

                OrderItem orderItemEntity = OrderItem.Create(
                    matchedProductId,
                    marketplaceOrderItemDto.Barcode,
                    marketplaceOrderItemDto.Sku,
                    marketplaceOrderItemDto.Quantity,
                    marketplaceOrderItemDto.UnitPrice,
                    marketplaceOrderItemDto.VatRate,
                    marketplaceOrderItemDto.CommissionRate.GetValueOrDefault(),
                    marketplaceOrderItemDto.PlatformCoveredDiscount,
                    marketplaceOrderItemDto.SellerCoveredDiscount,
                    marketplaceOrderItemDto.OrderItemStatus
                );

                orderItemEntities.Add(orderItemEntity);
            }

            return orderItemEntities;
        }
    }
}
