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
                .ForMember(dest => dest.VatRate, opt => opt.MapFrom(src => src.VatRate));

        }

    }
}
