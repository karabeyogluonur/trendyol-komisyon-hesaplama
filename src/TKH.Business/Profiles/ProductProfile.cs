using AutoMapper;
using TKH.Business.Integrations.Dtos;
using TKH.Entities;

namespace TKH.Business.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<MarketplaceProductDto, Product>()
                .ForMember(dest => dest.LastUpdateDateTime, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.MarketplaceAccountId, opt => opt.Ignore())
                .ForMember(dest => dest.CommissionRate, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Attributes, opt => opt.Ignore())
                .ForMember(dest => dest.Prices, opt => opt.Ignore())
                .ForMember(dest => dest.Expenses, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore());
        }
    }
}
