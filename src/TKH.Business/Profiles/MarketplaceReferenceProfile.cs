using AutoMapper;
using TKH.Business.Integrations.Dtos;
using TKH.Entities;

namespace TKH.Business.Profiles
{
    public class MarketplaceReferenceProfile : Profile
    {
        public MarketplaceReferenceProfile()
        {
            CreateMap<MarketplaceCategoryDto, Category>()
                .ForMember(dest => dest.MarketplaceType, opt => opt.Ignore())
                .ForMember(dest => dest.Attributes, opt => opt.Ignore())
                .ForMember(dest => dest.DefaultCommissionRate, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<MarketplaceCategoryAttributeDto, CategoryAttribute>()
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.AttributeValues, opt => opt.MapFrom(src => src.AttributeValues));

            CreateMap<MarketplaceAttributeValueDto, AttributeValue>()
                .ForMember(dest => dest.CategoryAttributeId, opt => opt.Ignore());
        }
    }
}
