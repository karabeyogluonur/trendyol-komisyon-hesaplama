using AutoMapper;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Entities;

namespace TKH.Business.Features.Orders.Mappings
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<MarketplaceOrderItemDto, OrderItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore());

            CreateMap<MarketplaceOrderDto, Order>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.LastUpdateDateTime, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src =>
                    src.OrderDate.Kind == DateTimeKind.Utc
                        ? src.OrderDate
                        : DateTime.SpecifyKind(src.OrderDate, DateTimeKind.Utc)))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.MarketplaceAccount, opt => opt.Ignore());
        }
    }
}
