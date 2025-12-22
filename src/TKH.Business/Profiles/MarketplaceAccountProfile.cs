using AutoMapper;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Core.Utilities.Security.Encryption;
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
            CreateMap<MarketplaceAccount, MarketplaceAccountConnectionDetailsDto>()
            .ForMember(dest => dest.ApiSecretKey, opt => opt.MapFrom((src, dest, destMember, context) =>
            {
                var cipherService = (ICipherService)context.Items["CipherService"];
                return cipherService.Decrypt(src.ApiSecretKey);
            }));
        }
    }
}
