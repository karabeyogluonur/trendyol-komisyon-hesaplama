using AutoMapper;
using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Presentation.Features.MarketplaceAccounts.Models;

namespace TKH.Presentation.Features.MarketplaceAccounts.Mappings
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
