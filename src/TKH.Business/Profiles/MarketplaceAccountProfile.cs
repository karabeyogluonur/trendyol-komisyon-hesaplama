using AutoMapper;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Entities.Concrete;

namespace TKH.Business.Profiles
{
    public class MarketplaceAccountProfile : Profile
    {
        public MarketplaceAccountProfile()
        {
            CreateMap<MarketplaceAccount, MarketplaceAccountListDto>()
                .ForMember(dest => dest.MarketplaceTypeName, opt => opt.MapFrom(src => src.MarketplaceType.ToString()));

            CreateMap<MarketplaceAccountAddDto, MarketplaceAccount>();
            CreateMap<MarketplaceAccount, MarketplaceAccountUpdateDto>();
        }
    }
}
