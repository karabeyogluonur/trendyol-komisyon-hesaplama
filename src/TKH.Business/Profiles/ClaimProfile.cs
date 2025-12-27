using AutoMapper;
using TKH.Business.Integrations.Dtos;
using TKH.Entities;

namespace TKH.Business.Mappers
{
    public class ClaimProfile : Profile
    {
        public ClaimProfile()
        {
            CreateMap<MarketplaceClaimItemDto, ClaimItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ClaimId, opt => opt.Ignore())
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.Claim, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            CreateMap<MarketplaceClaimDto, Claim>()
                .ForMember(dest => dest.ClaimItems, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.MarketplaceAccountId, opt => opt.Ignore())
                .ForMember(dest => dest.MarketplaceAccount, opt => opt.Ignore())

                .ForMember(dest => dest.ClaimDate, opt => opt.MapFrom(src =>
                    src.ClaimDate.Kind == DateTimeKind.Utc
                        ? src.ClaimDate
                        : DateTime.SpecifyKind(src.ClaimDate, DateTimeKind.Utc)))

                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src =>
                    src.OrderDate.Kind == DateTimeKind.Utc
                        ? src.OrderDate
                        : DateTime.SpecifyKind(src.OrderDate, DateTimeKind.Utc)))

                .ForMember(dest => dest.LastUpdateDateTime, opt => opt.MapFrom(src =>
                    src.LastUpdateDateTime.Kind == DateTimeKind.Utc
                        ? src.LastUpdateDateTime
                        : DateTime.SpecifyKind(src.LastUpdateDateTime, DateTimeKind.Utc)));
        }
    }
}
