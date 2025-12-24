using AutoMapper;
using TKH.Business.Integrations.Dtos;
using TKH.Business.Integrations.Providers.Trendyol.Enums;
using TKH.Business.Integrations.Providers.Trendyol.Models;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Providers.Trendyol.Profiles
{
    public class TrendyolOrderProfile : Profile
    {
        public TrendyolOrderProfile()
        {
            CreateMap<TrendyolOrderLine, MarketplaceOrderItemDto>()
                .ForMember(dest => dest.CommissionRate, opt => opt.MapFrom(src => src.Commission))
                .ForMember(dest => dest.MarketplaceDiscount, opt => opt.MapFrom(src => src.LineTyDiscount))
                .ForMember(dest => dest.SellerDiscount, opt => opt.MapFrom(src => src.LineSellerDiscount))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapOrderItemStatus(src.OrderLineItemStatusName)));

            CreateMap<TrendyolOrderContent, MarketplaceOrderDto>()
                .ForMember(dest => dest.MarketplaceOrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.GrossAmount))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeMilliseconds(src.OrderDate).DateTime))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Lines))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapOrderStatus(src.Status)))
                .ForMember(dest => dest.MarketplaceAccountId, opt => opt.Ignore());
        }


        private OrderStatus MapOrderStatus(TrendyolOrderStatus status)
        {
            return status switch
            {
                TrendyolOrderStatus.Awaiting => OrderStatus.Other,
                TrendyolOrderStatus.Created => OrderStatus.Other,
                TrendyolOrderStatus.Picking => OrderStatus.Other,
                TrendyolOrderStatus.Shipped => OrderStatus.Other,
                TrendyolOrderStatus.Delivered => OrderStatus.Delivered,
                TrendyolOrderStatus.Cancelled => OrderStatus.Other,
                TrendyolOrderStatus.Returned => OrderStatus.Other,
                _ => OrderStatus.Other
            };
        }

        private OrderItemStatus MapOrderItemStatus(string statusName)
        {
            if (string.IsNullOrEmpty(statusName)) return OrderItemStatus.Delivered;

            var normalized = statusName.ToLowerInvariant();

            if (normalized.Contains("cancel") || normalized.Contains("supply")) return OrderItemStatus.Other;
            if (normalized.Contains("return")) return OrderItemStatus.Other;
            if (normalized.Contains("ship")) return OrderItemStatus.Other;
            if (normalized.Contains("deliver")) return OrderItemStatus.Delivered;

            return OrderItemStatus.Other;
        }
    }
}
