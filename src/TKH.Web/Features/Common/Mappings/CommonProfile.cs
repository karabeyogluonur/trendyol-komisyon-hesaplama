using AutoMapper;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Web.Features.Common.Models;

namespace TKH.Web.Features.Common.Mappings
{
    public class CommonProfile : Profile
    {
        public CommonProfile()
        {
            CreateMap<MarketplaceDefaultsDto, MarketplaceDefaultsViewModel>();
        }
    }
}
