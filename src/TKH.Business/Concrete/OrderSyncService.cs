using AutoMapper;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;
        private readonly IMapper _mapper;

        public OrderSyncService(
            IUnitOfWork unitOfWork,
            MarketplaceProviderFactory marketplaceProviderFactory,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = _unitOfWork.GetRepository<Order>();
            _productRepository = _unitOfWork.GetRepository<Product>();
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
            List<string> incomingMarketplaceOrderNumberList = marketplaceOrderDtoList
                .Select(marketplaceOrderDto => marketplaceOrderDto.MarketplaceOrderNumber)
                .Where(marketplaceOrderNumber => !string.IsNullOrEmpty(marketplaceOrderNumber))
                .ToList();

            List<Order> existingOrderList = await _orderRepository.GetAllAsync(
                order => order.MarketplaceAccountId == marketplaceAccountId && incomingMarketplaceOrderNumberList.Contains(order.MarketplaceOrderNumber),
                includes: orderQuery => orderQuery.OrderItems
            );

            List<string> allMarketplaceOrderItemBarcodeList = marketplaceOrderDtoList
                .SelectMany(marketplaceOrderDto => marketplaceOrderDto.Items)
                .Select(marketplaceOrderItemDto => marketplaceOrderItemDto.Barcode)
                .Distinct()
                .ToList();

            List<Product> relatedProductList = await _productRepository.GetAllAsync(
                product => product.MarketplaceAccountId == marketplaceAccountId && allMarketplaceOrderItemBarcodeList.Contains(product.Barcode)
            );

            List<Order> ordersToProcessList = new List<Order>();

            foreach (MarketplaceOrderDto marketplaceOrderDto in marketplaceOrderDtoList)
            {
                Order existingOrder = existingOrderList.FirstOrDefault(order => order.MarketplaceOrderNumber == marketplaceOrderDto.MarketplaceOrderNumber);

                if (existingOrder != null)
                {
                    _mapper.Map(marketplaceOrderDto, existingOrder);
                    existingOrder.LastUpdateDateTime = DateTime.UtcNow;

                    existingOrder.OrderItems.Clear();

                    foreach (MarketplaceOrderItemDto marketplaceOrderItemDto in marketplaceOrderDto.Items)
                    {
                        OrderItem newOrderItem = _mapper.Map<OrderItem>(marketplaceOrderItemDto);
                        Product matchedProduct = relatedProductList.FirstOrDefault(product => product.Barcode == marketplaceOrderItemDto.Barcode);

                        if (matchedProduct != null)
                            newOrderItem.ProductId = matchedProduct.Id;

                        existingOrder.OrderItems.Add(newOrderItem);
                    }

                    ordersToProcessList.Add(existingOrder);
                }
                else
                {
                    Order newOrder = _mapper.Map<Order>(marketplaceOrderDto);
                    newOrder.MarketplaceAccountId = marketplaceAccountId;
                    newOrder.LastUpdateDateTime = DateTime.UtcNow;

                    MatchOrderItemsWithProducts(newOrder, relatedProductList);

                    ordersToProcessList.Add(newOrder);
                }
            }

            _orderRepository.AddOrUpdate(ordersToProcessList);
            await _unitOfWork.SaveChangesAsync();
        }

        private void MatchOrderItemsWithProducts(Order orderEntity, List<Product> productList)
        {
            if (orderEntity.OrderItems == null)
                return;

            foreach (OrderItem orderItem in orderEntity.OrderItems)
            {
                Product matchedProduct = productList.FirstOrDefault(product => product.Barcode == orderItem.Barcode);

                if (matchedProduct != null)
                    orderItem.ProductId = matchedProduct.Id;
            }
        }
    }
}
