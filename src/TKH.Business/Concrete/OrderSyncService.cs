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
    public class OrderSyncService : IOrderSyncService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;
        private readonly IMapper _mapper;

        public OrderSyncService(
            IServiceScopeFactory serviceScopeFactory,
            MarketplaceProviderFactory marketplaceProviderFactory,
            IMapper mapper)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _marketplaceProviderFactory = marketplaceProviderFactory;
            _mapper = mapper;
        }

        public async Task SyncOrdersFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto)
        {
            IMarketplaceOrderProvider marketplaceOrderProvider = _marketplaceProviderFactory.GetProvider<IMarketplaceOrderProvider>(marketplaceAccountConnectionDetailsDto.MarketplaceType);

            List<MarketplaceOrderDto> marketplaceOrderDtoBuffer = new List<MarketplaceOrderDto>(ApplicationDefaults.OrderBatchSize);

            await foreach (MarketplaceOrderDto incomingMarketplaceOrderDto in marketplaceOrderProvider.GetOrdersStreamAsync(marketplaceAccountConnectionDetailsDto))
            {
                marketplaceOrderDtoBuffer.Add(incomingMarketplaceOrderDto);

                if (marketplaceOrderDtoBuffer.Count >= ApplicationDefaults.OrderBatchSize)
                {
                    await ProcessOrderBatchAsync(marketplaceOrderDtoBuffer, marketplaceAccountConnectionDetailsDto.Id);
                    marketplaceOrderDtoBuffer.Clear();
                }
            }

            if (marketplaceOrderDtoBuffer.Count > 0)
                await ProcessOrderBatchAsync(marketplaceOrderDtoBuffer, marketplaceAccountConnectionDetailsDto.Id);
        }

        private async Task ProcessOrderBatchAsync(List<MarketplaceOrderDto> marketplaceOrderDtoList, int marketplaceAccountId)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork scopedUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<Order> scopedOrderRepository = scopedUnitOfWork.GetRepository<Order>();
                IRepository<Product> scopedProductRepository = scopedUnitOfWork.GetRepository<Product>();

                List<string> incomingShipmentIdList = marketplaceOrderDtoList
                    .Select(dto => dto.ExternalShipmentId)
                    .Where(id => !string.IsNullOrEmpty(id))
                    .ToList();

                List<string> allMarketplaceProductCodes = marketplaceOrderDtoList
                    .SelectMany(dto => dto.Items)
                    .Select(item => item.ExternalProductCode)
                    .Where(code => !string.IsNullOrEmpty(code))
                    .Distinct()
                    .ToList();

                IList<Product> relatedProductList = await scopedProductRepository.GetAllAsync(
                    predicate: product => product.MarketplaceAccountId == marketplaceAccountId &&
                                          allMarketplaceProductCodes.Contains(product.ExternalProductCode),
                    disableTracking: true,
                    ignoreQueryFilters: true
                );

                Dictionary<string, int> codeToLocalIdMap = relatedProductList
                    .GroupBy(product => product.ExternalProductCode)
                    .ToDictionary(group => group.Key, group => group.First().Id);

                IList<Order> existingOrderList = await scopedOrderRepository.GetAllAsync(
                    predicate: order => order.MarketplaceAccountId == marketplaceAccountId && incomingShipmentIdList.Contains(order.ExternalShipmentId),
                    include: source => source.Include(order => order.OrderItems),
                    disableTracking: false,
                    ignoreQueryFilters: true
                );

                List<Order> newOrdersToAdd = new List<Order>();

                foreach (MarketplaceOrderDto marketplaceOrderDto in marketplaceOrderDtoList)
                {
                    Order? existingOrder = existingOrderList.FirstOrDefault(order => order.ExternalShipmentId == marketplaceOrderDto.ExternalShipmentId);

                    if (existingOrder is not null)
                    {
                        _mapper.Map(marketplaceOrderDto, existingOrder);
                        existingOrder.LastUpdateDateTime = DateTime.UtcNow;
                        SyncOrderItems(existingOrder, marketplaceOrderDto.Items, codeToLocalIdMap);
                    }
                    else
                    {
                        Order newOrder = _mapper.Map<Order>(marketplaceOrderDto);
                        newOrder.MarketplaceAccountId = marketplaceAccountId;
                        newOrder.LastUpdateDateTime = DateTime.UtcNow;
                        SyncOrderItems(newOrder, marketplaceOrderDto.Items, codeToLocalIdMap);

                        newOrdersToAdd.Add(newOrder);
                    }
                }

                if (newOrdersToAdd.Count > 0)
                    await scopedOrderRepository.InsertAsync(newOrdersToAdd);

                await scopedUnitOfWork.SaveChangesAsync();
            }
        }

        private void SyncOrderItems(Order order, List<MarketplaceOrderItemDto> marketplaceItems, Dictionary<string, int> codeToLocalIdMap)
        {
            if (marketplaceItems is null || !marketplaceItems.Any())
                return;

            if (order.OrderItems is null)
                order.OrderItems = new List<OrderItem>();
            else
                order.OrderItems.Clear();

            foreach (MarketplaceOrderItemDto itemDto in marketplaceItems)
            {
                OrderItem orderItem = _mapper.Map<OrderItem>(itemDto);

                if (!string.IsNullOrEmpty(itemDto.ExternalProductCode) && codeToLocalIdMap.TryGetValue(itemDto.ExternalProductCode, out int productId))
                    orderItem.ProductId = productId;
                else
                    orderItem.ProductId = null;

                order.OrderItems.Add(orderItem);
            }
        }
    }
}
