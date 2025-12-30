using AutoMapper;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Entities;

namespace TKH.Business.Features.ShipmentTransactions.Mappings
{
    public class ShipmentTransactionProfile : Profile
    {
        public ShipmentTransactionProfile()
        {
            CreateMap<MarketplaceShipmentTransactionDto, ShipmentTransaction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
