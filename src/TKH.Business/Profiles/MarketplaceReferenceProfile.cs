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
                .ForMember(category => category.MarketplaceType, option => option.Ignore())
                .ForMember(category => category.CategoryAttributes, option => option.Ignore())
                .ForMember(category => category.DefaultCommissionRate, option => option.Ignore())
                .ForMember(category => category.Id, option => option.Ignore());

            CreateMap<MarketplaceCategoryAttributeDto, CategoryAttribute>()
                .ForMember(categoryAttribute => categoryAttribute.CategoryId, option => option.Ignore())
                .ForMember(categoryAttribute => categoryAttribute.Category, option => option.Ignore())
                .ForMember(categoryAttribute => categoryAttribute.AttributeValues, option => option.MapFrom(src => src.AttributeValues))
                .ForMember(categoryAttribute => categoryAttribute.Id, option => option.Ignore());

            CreateMap<MarketplaceAttributeValueDto, AttributeValue>()
                .ForMember(attributeValue => attributeValue.CategoryAttributeId, option => option.Ignore())
                .ForMember(attributeValue => attributeValue.CategoryAttribute, option => option.Ignore())
                .ForMember(attributeValue => attributeValue.Id, option => option.Ignore());
        }
    }
}
