using AutoMapper;
using TKH.Business.Integrations.Dtos;
using TKH.Business.Integrations.Providers.Trendyol.Models;

namespace TKH.Business.Integrations.Providers.Trendyol.Profiles
{
    public class TrendyolProductProfile : Profile
    {
        public TrendyolProductProfile()
        {
            CreateMap<TrendyolProductContent, MarketplaceProductDto>()
                .ForMember(dest => dest.MarketplaceProductId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ModelCode, opt => opt.MapFrom(src => src.ProductMainId))
                .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.Barcode))
                .ForMember(dest => dest.StockCode, opt => opt.MapFrom(src => src.StockCode))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.VatRate, opt => opt.MapFrom(src => src.VatRate))
                .ForMember(dest => dest.MarketplaceCategoryId, opt => opt.MapFrom(src => src.PimCategoryId.HasValue ? src.PimCategoryId.Value.ToString() : string.Empty))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes))
                .ForMember(dest => dest.CommissionRate, opt => opt.Ignore());

            CreateMap<TrendyolProductAttribute, MarketplaceProductAttributeDto>()
                .ForMember(dest => dest.MarketplaceAttributeId, opt => opt.MapFrom(src => src.AttributeId.ToString()))
                .ForMember(dest => dest.AttributeName, opt => opt.MapFrom(src => src.AttributeName))
                .ForMember(dest => dest.MarketplaceValueId, opt => opt.MapFrom(src => src.AttributeValueId.HasValue ? src.AttributeValueId.Value.ToString() : null))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.AttributeValue));
        }
    }
}
