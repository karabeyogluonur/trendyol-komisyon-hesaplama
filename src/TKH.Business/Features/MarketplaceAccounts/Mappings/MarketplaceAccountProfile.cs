using AutoMapper;

using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Entities;

namespace TKH.Business.Features.MarketplaceAccounts.Mappings
{
    public class MarketplaceAccountProfile : Profile
    {
        public MarketplaceAccountProfile()
        {
            CreateMap<MarketplaceAccount, MarketplaceAccountSummaryDto>();
            CreateMap<MarketplaceAccountCreateDto, MarketplaceAccount>();
            CreateMap<MarketplaceAccount, MarketplaceAccountUpdateDto>();
            CreateMap<MarketplaceAccount, MarketplaceAccountDetailsDto>();
            CreateMap<MarketplaceAccount, MarketplaceAccountConnectionDetailsDto>();
        }
    }
}
