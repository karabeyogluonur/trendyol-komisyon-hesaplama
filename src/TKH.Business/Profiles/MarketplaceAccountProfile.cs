using AutoMapper;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Entities;

namespace TKH.Business.Profiles
{
    public class MarketplaceAccountProfile : Profile
    {
        public MarketplaceAccountProfile()
        {
            CreateMap<MarketplaceAccount, MarketplaceAccountSummaryDto>();
            CreateMap<MarketplaceAccountAddDto, MarketplaceAccount>();
            CreateMap<MarketplaceAccount, MarketplaceAccountUpdateDto>();
            CreateMap<MarketplaceAccount, MarketplaceAccountDetailsDto>();
            CreateMap<MarketplaceAccount, MarketplaceAccountConnectionDetailsDto>();
        }
    }
}
