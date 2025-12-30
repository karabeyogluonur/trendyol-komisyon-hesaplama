using AutoMapper;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Business.Integrations.Providers.Trendyol.Models;

namespace TKH.Business.Mappers
{
    public class TrendyolReferenceProfile : Profile
    {
        public TrendyolReferenceProfile()
        {
            CreateMap<TrendyolCategoryContent, MarketplaceCategoryDto>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.ParentExternalId, opt => opt.MapFrom(src => src.ParentId.HasValue ? src.ParentId.Value.ToString() : null))
                .ForMember(dest => dest.IsLeaf, opt => opt.MapFrom(src => src.SubCategories == null || src.SubCategories.Count == 0));

            CreateMap<TrendyolCategoryAttributeContent, MarketplaceCategoryAttributeDto>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Attribute.Id.ToString()))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Attribute.Name))
                .ForMember(dest => dest.IsVariant, opt => opt.MapFrom(src => src.Varianter))
                .ForMember(dest => dest.Values, opt => opt.MapFrom(src => src.AttributeValues));

            CreateMap<TrendyolAttributeValue, MarketplaceAttributeValueDto>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Name));
        }
    }
}
