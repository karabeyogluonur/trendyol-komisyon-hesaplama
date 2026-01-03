using AutoMapper;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Presentation.Features.Common.Models;

namespace TKH.Presentation.Features.Common.Mappings
{
    public class CommonProfile : Profile
    {
        public CommonProfile()
        {
            CreateMap<MarketplaceDefaultsDto, MarketplaceDefaultsViewModel>();
        }
    }
}
