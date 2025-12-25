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

                List<string> incomingMarketplaceOrderNumberList = marketplaceOrderDtoList
                    .Select(marketplaceOrderDto => marketplaceOrderDto.MarketplaceOrderNumber)
                    .Where(marketplaceOrderNumber => !string.IsNullOrEmpty(marketplaceOrderNumber))
                    .ToList();

                List<string> allMarketplaceOrderItemBarcodeList = marketplaceOrderDtoList
                    .SelectMany(marketplaceOrderDto => marketplaceOrderDto.Items)
                    .Select(marketplaceOrderItemDto => marketplaceOrderItemDto.Barcode)
                    .Distinct()
                    .ToList();

                IList<Product> relatedProductList = await scopedProductRepository.GetAllAsync(
                    predicate: product => product.MarketplaceAccountId == marketplaceAccountId && allMarketplaceOrderItemBarcodeList.Contains(product.Barcode),
                    disableTracking: true
                );

                Dictionary<string, int> productBarcodeToIdMap = relatedProductList
                    .GroupBy(product => product.Barcode)
                    .ToDictionary(group => group.Key, group => group.First().Id);

                IList<Order> existingOrderList = await scopedOrderRepository.GetAllAsync(
                    predicate: order => order.MarketplaceAccountId == marketplaceAccountId && incomingMarketplaceOrderNumberList.Contains(order.MarketplaceOrderNumber),
                    include: source => source.Include(order => order.OrderItems),
                    disableTracking: false
                );

                List<Order> newOrdersToAdd = new List<Order>();

                foreach (MarketplaceOrderDto marketplaceOrderDto in marketplaceOrderDtoList)
                {
                    Order? existingOrder = existingOrderList.FirstOrDefault(order => order.MarketplaceOrderNumber == marketplaceOrderDto.MarketplaceOrderNumber);

                    if (existingOrder is not null)
                    {
                        _mapper.Map(marketplaceOrderDto, existingOrder);
                        existingOrder.LastUpdateDateTime = DateTime.UtcNow;

                        SyncOrderItems(existingOrder, marketplaceOrderDto.Items, productBarcodeToIdMap);
                    }
                    else
                    {
                        Order newOrder = _mapper.Map<Order>(marketplaceOrderDto);
                        newOrder.MarketplaceAccountId = marketplaceAccountId;
                        newOrder.LastUpdateDateTime = DateTime.UtcNow;

                        SyncOrderItems(newOrder, marketplaceOrderDto.Items, productBarcodeToIdMap);

                        newOrdersToAdd.Add(newOrder);
                    }
                }

                if (newOrdersToAdd.Count > 0)
                    await scopedOrderRepository.InsertAsync(newOrdersToAdd);

                await scopedUnitOfWork.SaveChangesAsync();
            }
        }

        private void SyncOrderItems(Order order, List<MarketplaceOrderItemDto> marketplaceOrderItemDtos, Dictionary<string, int> productBarcodeToIdMap)
        {
            if (marketplaceOrderItemDtos is null)
                return;

            if (order.OrderItems is null)
                order.OrderItems = new List<OrderItem>();
            else
                order.OrderItems.Clear();

            foreach (MarketplaceOrderItemDto marketplaceOrderItemDto in marketplaceOrderItemDtos)
            {
                OrderItem newOrderItem = _mapper.Map<OrderItem>(marketplaceOrderItemDto);

                if (productBarcodeToIdMap.TryGetValue(marketplaceOrderItemDto.Barcode, out int productId))
                    newOrderItem.ProductId = productId;

                order.OrderItems.Add(newOrderItem);
            }
        }
    }
}
