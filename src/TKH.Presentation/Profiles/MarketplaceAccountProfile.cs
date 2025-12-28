using AutoMapper;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Presentation.Models.MarketplaceAccount;

namespace TKH.Presentation.Profiles
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
