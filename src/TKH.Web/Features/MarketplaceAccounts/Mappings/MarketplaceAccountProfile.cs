using AutoMapper;
using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Web.Features.MarketplaceAccounts.Models;

namespace TKH.Web.Features.MarketplaceAccounts.Mappings
{
    public class MarketplaceAccountProfile : Profile
    {
        public MarketplaceAccountProfile()
        {
            CreateMap<MarketplaceAccountAddViewModel, MarketplaceAccountAddDto>();
            CreateMap<MarketplaceAccountSummaryDto, MarketplaceAccountListViewModel>();
            CreateMap<MarketplaceAccountDetailsDto, MarketplaceAccountUpdateViewModel>();
            CreateMap<MarketplaceAccountUpdateViewModel, MarketplaceAccountUpdateDto>();
            CreateMap<MarketplaceAccountSummaryDto, MarketplaceAccountSelectorItemViewModel>();
        }
    }
}
