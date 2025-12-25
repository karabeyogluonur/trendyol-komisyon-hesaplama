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
            .ForMember(dest => dest.ProductAttributes, opt => opt.Ignore())
                .ForMember(dest => dest.ProductPrices, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.CommissionRate, opt => opt.Ignore())
                .ForMember(dest => dest.ProductExpenses, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdateDateTime, opt => opt.Ignore());
        }
    }
}
