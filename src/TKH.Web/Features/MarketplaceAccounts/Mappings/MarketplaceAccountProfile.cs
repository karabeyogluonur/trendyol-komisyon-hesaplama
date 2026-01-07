using AutoMapper;
using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Core.Common.Constants;
using TKH.Web.Features.MarketplaceAccounts.Models;

namespace TKH.Web.Features.MarketplaceAccounts.Mappings
{
    public class MarketplaceAccountProfile : Profile
    {
        public MarketplaceAccountProfile()
        {
            CreateMap<MarketplaceAccountAddViewModel, MarketplaceAccountAddDto>();

            CreateMap<MarketplaceAccountSummaryDto, MarketplaceAccountListViewModel>()
                .ForMember(dest => dest.IsDemo, opt => opt.MapFrom(src => src.MerchantId == ApplicationDefaults.DemoAccountMerchantId ? true : false));

            CreateMap<MarketplaceAccountDetailsDto, MarketplaceAccountUpdateViewModel>()
                .ForMember(dest => dest.IsDemo, opt => opt.MapFrom(src => src.MerchantId == ApplicationDefaults.DemoAccountMerchantId ? true : false));

            CreateMap<MarketplaceAccountUpdateViewModel, MarketplaceAccountUpdateDto>();

            CreateMap<MarketplaceAccountSummaryDto, MarketplaceAccountSelectorItemViewModel>()
                .ForMember(dest => dest.IsDemo, opt => opt.MapFrom(src => src.MerchantId == ApplicationDefaults.DemoAccountMerchantId ? true : false));
        }
    }
}
